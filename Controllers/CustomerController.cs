using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GqeberhaPharmacy.Data;
using GqeberhaPharmacy.Models;
using GqeberhaPharmacy.Services;
using System.ComponentModel.DataAnnotations;

namespace GqeberhaPharmacy.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public CustomerController(ApplicationDbContext context, EmailService emailService, PdfService pdfService)
        {
            _context = context;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound();

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Doctor)
                .Where(p => p.CustomerId == customer.Id)
                .ToListAsync();

            return View(prescriptions);
        }

        // Upload Prescription
        [HttpGet]
        public IActionResult UploadPrescription()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPrescription(UploadPrescriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null)
                    return NotFound();

                // Handle PDF upload
                string pdfPath = null;
                if (model.PrescriptionPdf != null && model.PrescriptionPdf.ContentType == "application/pdf")
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "prescriptions");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PrescriptionPdf.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PrescriptionPdf.CopyToAsync(stream);
                    }
                    pdfPath = $"/uploads/prescriptions/{uniqueFileName}";
                }

                var order = new PrescriptionOrder
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customer.Id,
                    OrderDate = DateTime.Now,
                    Status = 0
                };

                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyOrders));
            }

            return View(model);
        }

        // View My Orders
        public async Task<IActionResult> MyOrders()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound();

            var orders = await _context.PrescriptionOrders
                .Include(po => po.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .Where(po => po.CustomerId == customer.Id)
                .ToListAsync();

            return View(orders);
        }

        // View Order Details
        public async Task<IActionResult> OrderDetail(string id)
        {
            var order = await _context.PrescriptionOrders
                .Include(po => po.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .ThenInclude(m => m.MedicationIngredients)
                .ThenInclude(mi => mi.ActiveIngredient)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Request Prescription Repeat
        [HttpPost]
        public async Task<IActionResult> RequestRepeat(string prescriptionItemId)
        {
            var item = await _context.PrescriptionItems
                .Include(pi => pi.Prescription)
                .FirstOrDefaultAsync(pi => pi.Id == prescriptionItemId);

            if (item == null)
                return NotFound();

            var repeatsLeft = item.NumberOfRepeats - item.RepeatsUsed;

            if (repeatsLeft <= 0)
            {
                return Json(new { success = false, message = "No repeats left for this medication" });
            }

            // Create a new prescription order for the repeat
            var customer = item.Prescription.Customer;
            var order = new PrescriptionOrder
            {
                Id = Guid.NewGuid().ToString(),
                PrescriptionId = item.PrescriptionId,
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                Status = 0
            };

            _context.Add(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Repeat requested successfully", orderId = order.Id });
        }

        // View Repeats
        public async Task<IActionResult> ViewRepeats()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound();

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .Where(p => p.CustomerId == customer.Id)
                .ToListAsync();

            var repeatsInfo = prescriptions.SelectMany(p => p.Items).Select(pi => new
            {
                pi.Id,
                MedicationName = pi.Medication.Name,
                RepeatsUsed = pi.RepeatsUsed,
                TotalRepeats = pi.NumberOfRepeats,
                RepeatsLeft = pi.NumberOfRepeats - pi.RepeatsUsed
            }).ToList();

            return View(repeatsInfo);
        }

        // Generate Report
        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime fromDate, DateTime toDate, string groupBy = "medication")
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound();

            var dispensedItems = await _context.PrescriptionOrders
                .Include(po => po.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .Where(po => po.CustomerId == customer.Id && 
                       po.CollectedDate >= fromDate && 
                       po.CollectedDate <= toDate)
                .SelectMany(po => po.Prescription.Items.Select(pi => (
                    Date: po.CollectedDate ?? DateTime.Now,
                    Medication: pi.Medication.Name,
                    Quantity: pi.Quantity,
                    Price: pi.Medication.SalesPrice
                )))
                .ToListAsync();

            var pdf = _pdfService.GenerateCustomerReportPdf(
                $"{customer.FirstName} {customer.LastName}",
                dispensedItems
            );

            return File(pdf, "application/pdf", $"Prescriptions_Report_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // Update Profile
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string id, [Bind("FirstName,LastName,CellphoneNumber,Allergies")] Customer customer)
        {
            if (id != customer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(MyProfile));
            }
            return View(customer);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }

    public class UploadPrescriptionViewModel
    {
        [Required]
        [Display(Name = "Prescription PDF")]
        public IFormFile PrescriptionPdf { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
