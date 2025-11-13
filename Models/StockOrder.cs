namespace GqeberhaPharmacy.Models
{
    public class StockOrder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0; // 0: Pending, 1: Received
        public DateTime? ReceivedDate { get; set; }

        // Foreign keys
        public string SupplierId { get; set; } = string.Empty;
        public Supplier Supplier { get; set; } = null!;
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<StockOrderItem> Items { get; set; } = new List<StockOrderItem>();
    }
}
