using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GqeberhaPharmacy.Data;
using GqeberhaPharmacy.Models;
using GqeberhaPharmacy.Services;
using System.ComponentModel.DataAnnotations;

namespace GqeberhaPharmacy.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public ManagerController(ApplicationDbContext context, EmailService emailService, PdfService pdfService)
        {
            _context = context;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var pharmacies = await _context.Pharmacies.ToListAsync();
            return View(pharmacies);
        }

        // Pharmacy Management
        public async Task<IActionResult> PharmacyDetails(string id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy == null)
                return NotFound();
            return View(pharmacy);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePharmacy(string id, [Bind("Name,HealthCouncilRegistrationNumber,PhysicalAddress,ContactNumber,EmailAddress,WebsiteUrl")] Pharmacy pharmacy)
        {
            if (id != pharmacy.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    pharmacy.UpdatedDate = DateTime.Now;
                    _context.Update(pharmacy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PharmacyExists(pharmacy.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(PharmacyDetails), new { id = pharmacy.Id });
            }
            return View(pharmacy);
        }

        // Medication Management
        public async Task<IActionResult> ManageMedications(string pharmacyId)
        {
            var medications = await _context.Medications
                .Include(m => m.DosageForm)
                .Include(m => m.Supplier)
                .Where(m => m.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(medications);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedication(string pharmacyId, [Bind("Name,Schedule,SalesPrice,ReorderLevel,QuantityOnHand,DosageFormId,SupplierId")] Medication medication)
        {
            medication.Id = Guid.NewGuid().ToString();
            medication.PharmacyId = pharmacyId;
            medication.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(medication);
                try
                {
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Medication created successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMedicationStock(string medicationId, int quantity, string action = "set")
        {
            var medication = await _context.Medications.FindAsync(medicationId);
            if (medication == null)
                return NotFound();

            if (action == "set")
                medication.QuantityOnHand = quantity;
            else if (action == "increment")
                medication.QuantityOnHand += quantity;

            medication.UpdatedDate = DateTime.Now;
            _context.Update(medication);
            await _context.SaveChangesAsync();

            return Json(new { success = true, newQuantity = medication.QuantityOnHand });
        }

        // Active Ingredient Management
        public async Task<IActionResult> ManageActiveIngredients(string pharmacyId)
        {
            var ingredients = await _context.ActiveIngredients
                .Where(ai => ai.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(ingredients);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActiveIngredient(string pharmacyId, [Bind("Name,Description")] ActiveIngredient ingredient)
        {
            ingredient.Id = Guid.NewGuid().ToString();
            ingredient.PharmacyId = pharmacyId;
            ingredient.CreatedDate = DateTime.Now;

            _context.Add(ingredient);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Active ingredient created successfully" });
        }

        // Dosage Form Management
        public async Task<IActionResult> ManageDosageForms(string pharmacyId)
        {
            var forms = await _context.DosageForms
                .Where(df => df.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(forms);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDosageForm(string pharmacyId, [Bind("Name,Description")] DosageForm form)
        {
            form.Id = Guid.NewGuid().ToString();
            form.PharmacyId = pharmacyId;
            form.CreatedDate = DateTime.Now;

            _context.Add(form);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Dosage form created successfully" });
        }

        // Supplier Management
        public async Task<IActionResult> ManageSuppliers(string pharmacyId)
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(suppliers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(string pharmacyId, [Bind("Name,ContactPersonName,ContactPersonEmail,ContactPersonPhone,PhysicalAddress")] Supplier supplier)
        {
            supplier.Id = Guid.NewGuid().ToString();
            supplier.PharmacyId = pharmacyId;
            supplier.CreatedDate = DateTime.Now;

            _context.Add(supplier);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Supplier created successfully" });
        }

        // Doctor Management
        public async Task<IActionResult> ManageDoctors(string pharmacyId)
        {
            var doctors = await _context.Doctors
                .Where(d => d.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(string pharmacyId, [Bind("FirstName,LastName,PracticeNumber,ContactNumber,EmailAddress")] Doctor doctor)
        {
            doctor.Id = Guid.NewGuid().ToString();
            doctor.PharmacyId = pharmacyId;
            doctor.CreatedDate = DateTime.Now;

            _context.Add(doctor);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Doctor created successfully" });
        }

        // Pharmacist Management
        public async Task<IActionResult> ManagePharmacists(string pharmacyId)
        {
            var pharmacists = await _context.Pharmacists
                .Include(p => p.User)
                .Where(p => p.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(pharmacists);
        }

        // Stock Order Management
        public async Task<IActionResult> StockOrders(string pharmacyId)
        {
            var orders = await _context.StockOrders
                .Include(so => so.Supplier)
                .Include(so => so.Items)
                .Where(so => so.PharmacyId == pharmacyId)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> LowStockMedications(string pharmacyId)
        {
            var lowStockMeds = await _context.Medications
                .Where(m => m.PharmacyId == pharmacyId && m.QuantityOnHand <= m.ReorderLevel + 10)
                .Include(m => m.Supplier)
                .Include(m => m.DosageForm)
                .ToListAsync();
            return View(lowStockMeds);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStockOrder(string pharmacyId, string supplierId, Dictionary<string, int> medicationQuantities)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                return NotFound();

            var stockOrder = new StockOrder
            {
                Id = Guid.NewGuid().ToString(),
                OrderNumber = $"SO-{DateTime.Now:yyyyMMddHHmmss}",
                SupplierId = supplierId,
                PharmacyId = pharmacyId,
                OrderDate = DateTime.Now
            };

            var medicationsList = "<ul>";
            foreach (var medQty in medicationQuantities)
            {
                var item = new StockOrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    StockOrderId = stockOrder.Id,
                    MedicationId = medQty.Key,
                    QuantityOrdered = medQty.Value
                };
                stockOrder.Items.Add(item);

                var med = await _context.Medications.FindAsync(medQty.Key);
                medicationsList += $"<li>{med?.Name} - Qty: {medQty.Value}</li>";
            }
            medicationsList += "</ul>";

            _context.Add(stockOrder);
            await _context.SaveChangesAsync();

            // Send email to supplier
            await _emailService.SendStockOrderEmailAsync(supplier.ContactPersonEmail, supplier.Name, stockOrder.OrderNumber, medicationsList);

            return Json(new { success = true, message = "Stock order created and email sent to supplier" });
        }

        [HttpPost]
        public async Task<IActionResult> MarkOrderReceived(string orderId)
        {
            var order = await _context.StockOrders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.Status = 1;
            order.ReceivedDate = DateTime.Now;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Order marked as received" });
        }

        // Reports
        [HttpGet]
        public async Task<IActionResult> GenerateStockTakeReport(string pharmacyId, string groupBy = "dosage")
        {
            var medications = await _context.Medications
                .Include(m => m.DosageForm)
                .Include(m => m.Supplier)
                .Where(m => m.PharmacyId == pharmacyId)
                .Select(m => (
                    MedicationName: m.Name,
                    DosageForm: m.DosageForm.Name,
                    QuantityOnHand: m.QuantityOnHand,
                    Schedule: m.Schedule,
                    Supplier: m.Supplier.Name
                ))
                .ToListAsync();

            var pharmacy = await _context.Pharmacies.FindAsync(pharmacyId);
            var pdf = _pdfService.GenerateStockTakePdf(pharmacy?.Name ?? "Pharmacy", medications, groupBy);

            return File(pdf, "application/pdf", $"StockTake_Report_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private bool PharmacyExists(string id)
        {
            return _context.Pharmacies.Any(e => e.Id == id);
        }
    }
}
