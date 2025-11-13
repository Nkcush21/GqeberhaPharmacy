using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GqeberhaPharmacy.Data;
using GqeberhaPharmacy.Models;
using GqeberhaPharmacy.Services;
using System.ComponentModel.DataAnnotations;

namespace GqeberhaPharmacy.Controllers
{
    [Authorize(Roles = "Pharmacist")]
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public PharmacistController(ApplicationDbContext context, EmailService emailService, PdfService pdfService)
        {
            _context = context;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Customer)
                .Include(p => p.Doctor)
                .ToListAsync();
            return View(prescriptions);
        }

        // Load Prescription
        public async Task<IActionResult> LoadPrescription()
        {
            var doctors = await _context.Doctors.ToListAsync();
            ViewBag.Doctors = doctors;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadPrescription(LoadPrescriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.IdNumber == model.PatientIdNumber);
                if (customer == null)
                {
                    ModelState.AddModelError("PatientIdNumber", "Customer not found");
                    return View(model);
                }

                var doctor = await _context.Doctors.FindAsync(model.DoctorId);
                if (doctor == null)
                {
                    ModelState.AddModelError("DoctorId", "Doctor not found");
                    return View(model);
                }

                // Get current user (pharmacist)
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);

                // Check for allergies
                var allergies = customer.Allergies?.Split(',');
                var allergyWarning = string.Empty;

                if (allergies != null && allergies.Length > 0)
                {
                    allergyWarning = $"WARNING: Patient has allergies to: {customer.Allergies}";
                }

                // Handle PDF upload
                string pdfPath = null;
                if (model.PrescriptionPdf != null)
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

                var prescription = new Prescription
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customer.Id,
                    DoctorId = model.DoctorId,
                    PharmacistId = pharmacist.Id,
                    PharmacyId = pharmacist.PharmacyId,
                    PrescriptionDate = model.PrescriptionDate,
                    PdfPath = pdfPath,
                    CreatedDate = DateTime.Now,
                    Status = 0
                };

                _context.Add(prescription);
                await _context.SaveChangesAsync();

                // Add prescription items
                foreach (var item in model.Items)
                {
                    var prescriptionItem = new PrescriptionItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        PrescriptionId = prescription.Id,
                        MedicationId = item.MedicationId,
                        Quantity = item.Quantity,
                        Instructions = item.Instructions,
                        NumberOfRepeats = item.NumberOfRepeats
                    };
                    _context.Add(prescriptionItem);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PrescriptionDetail), new { id = prescription.Id });
            }

            var doctors = await _context.Doctors.ToListAsync();
            ViewBag.Doctors = doctors;
            return View(model);
        }

        public async Task<IActionResult> PrescriptionDetail(string id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Customer)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .ThenInclude(m => m.MedicationIngredients)
                .ThenInclude(mi => mi.ActiveIngredient)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            return View(prescription);
        }

        // Dispense Prescription
        [HttpPost]
        public async Task<IActionResult> DispensePrescription(string prescriptionId)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var allergies = prescription.Customer.Allergies?.Split(',');
            var allergyWarning = string.Empty;

            if (allergies != null && allergies.Length > 0)
            {
                allergyWarning = $"WARNING: Patient has allergies to: {prescription.Customer.Allergies}";
            }

            // Check repeats and stock
            foreach (var item in prescription.Items)
            {
                if (item.RepeatsUsed >= item.NumberOfRepeats)
                {
                    return Json(new { success = false, message = "No repeats left for one or more medications" });
                }

                var medication = await _context.Medications.FindAsync(item.MedicationId);
                if (medication.QuantityOnHand < item.Quantity)
                {
                    return Json(new { success = false, message = $"Insufficient stock for {medication.Name}" });
                }
            }

            // Dispense medications
            prescription.Status = 2; // Dispensed
            prescription.DispensedDate = DateTime.Now;

            foreach (var item in prescription.Items)
            {
                item.RepeatsUsed++;

                var dispense = new PrescriptionDispense
                {
                    Id = Guid.NewGuid().ToString(),
                    PrescriptionItemId = item.Id,
                    PrescriptionId = prescriptionId,
                    PharmacistId = pharmacist.Id,
                    QuantityDispensed = item.Quantity,
                    DispenseDate = DateTime.Now
                };
                _context.Add(dispense);

                var medication = await _context.Medications.FindAsync(item.MedicationId);
                medication.QuantityOnHand -= item.Quantity;
                _context.Update(medication);
            }

            _context.Update(prescription);
            await _context.SaveChangesAsync();

            // Send notification email
            await _emailService.SendPrescriptionReadyNotificationAsync(
                prescription.Customer.EmailAddress,
                $"{prescription.Customer.FirstName} {prescription.Customer.LastName}",
                prescriptionId
            );

            return Json(new { success = true, message = "Prescription dispensed successfully", warning = allergyWarning });
        }

        // Reports
        [HttpGet]
        public async Task<IActionResult> GenerateDispenseReport(DateTime fromDate, DateTime toDate, string groupBy = "medication")
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var dispensedItems = await _context.PrescriptionDispenses
                .Include(pd => pd.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .Where(pd => pd.PharmacistId == pharmacist.Id && 
                       pd.DispenseDate >= fromDate && 
                       pd.DispenseDate <= toDate)
                .Select(pd => (
                    Date: pd.DispenseDate,
                    Medication: pd.Prescription.Items.First().Medication.Name,
                    Quantity: pd.QuantityDispensed,
                    Schedule: pd.Prescription.Items.First().Medication.Schedule.ToString()
                ))
                .ToListAsync();

            var pdf = _pdfService.GeneratePharmacistReportPdf(
                $"{pharmacist.FirstName} {pharmacist.LastName}",
                dispensedItems
            );

            return File(pdf, "application/pdf", $"Dispense_Report_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }

    public class LoadPrescriptionViewModel
    {
        [Required]
        public string PatientIdNumber { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public DateTime PrescriptionDate { get; set; }

        public IFormFile? PrescriptionPdf { get; set; }

        public List<PrescriptionItemViewModel> Items { get; set; } = new List<PrescriptionItemViewModel>();
    }

    public class PrescriptionItemViewModel
    {
        public string MedicationId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int NumberOfRepeats { get; set; }
    }
}
