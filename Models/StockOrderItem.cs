namespace GqeberhaPharmacy.Models
{
    public class StockOrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int QuantityOrdered { get; set; }

        // Foreign keys
        public string StockOrderId { get; set; } = string.Empty;
        public StockOrder StockOrder { get; set; } = null!;
        public string MedicationId { get; set; } = string.Empty;
        public Medication Medication { get; set; } = null!;
    }
}
