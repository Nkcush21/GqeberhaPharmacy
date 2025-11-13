namespace GqeberhaPharmacy.Models
{
    public class Supplier
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;
        public string ContactPersonEmail { get; set; } = string.Empty;
        public string? ContactPersonPhone { get; set; }
        public string? PhysicalAddress { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<StockOrder> StockOrders { get; set; } = new List<StockOrder>();
    }
}
