using GqeberhaPharmacy.Data;
using Microsoft.EntityFrameworkCore;

namespace GqeberhaPharmacy.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<T> GroupByDosageForm<T>(List<T> items) where T : class
        {
            return items.OrderBy(x => x.GetType().GetProperty("DosageForm")?.GetValue(x)).ToList();
        }

        public List<T> GroupBySchedule<T>(List<T> items) where T : class
        {
            return items.OrderBy(x => x.GetType().GetProperty("Schedule")?.GetValue(x)).ToList();
        }

        public List<T> GroupBySupplier<T>(List<T> items) where T : class
        {
            return items.OrderBy(x => x.GetType().GetProperty("Supplier")?.GetValue(x)).ToList();
        }

        public decimal CalculateAmountDue(List<(decimal Price, int Quantity)> items)
        {
            return items.Sum(x => x.Price * x.Quantity);
        }

        public async Task<decimal> CalculateOrderAmountAsync(string prescriptionOrderId)
        {
            var order = await _context.PrescriptionOrders
                .Include(po => po.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .FirstOrDefaultAsync(po => po.Id == prescriptionOrderId);

            if (order == null)
                return 0;

            decimal total = 0;
            foreach (var item in order.Prescription.Items)
            {
                total += item.Quantity * item.Medication.SalesPrice;
            }

            return total;
        }

        public async Task<List<(string MedicationName, int TotalDispensed)>> GetTopMedicationsAsync(int limit = 10)
        {
            var topMeds = await _context.PrescriptionDispenses
                .Include(pd => pd.Prescription)
                .ThenInclude(p => p.Items)
                .ThenInclude(pi => pi.Medication)
                .GroupBy(pd => pd.Prescription.Items.First().Medication.Name)
                .OrderByDescending(g => g.Sum(x => x.QuantityDispensed))
                .Take(limit)
                .Select(g => (
                    MedicationName: g.Key,
                    TotalDispensed: g.Sum(x => x.QuantityDispensed)
                ))
                .ToListAsync();

            return topMeds;
        }

        public async Task<int> GetTotalPrescriptionsAsync()
        {
            return await _context.Prescriptions.CountAsync();
        }

        public async Task<int> GetPendingPrescriptionsAsync()
        {
            return await _context.Prescriptions.CountAsync(p => p.Status == 0);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime fromDate, DateTime toDate)
        {
            var total = await _context.PrescriptionOrders
                .Where(po => po.CollectedDate >= fromDate && po.CollectedDate <= toDate && po.Status == 2)
                .SumAsync(po => po.AmountDue ?? 0);

            return total;
        }
    }
}
