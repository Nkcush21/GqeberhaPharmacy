namespace GqeberhaPharmacy.Models
{
    public class PrescriptionOrder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0; // 0: Pending, 1: Ready, 2: Collected
        public decimal? AmountDue { get; set; }
        public DateTime? ReadyDate { get; set; }
        public DateTime? CollectedDate { get; set; }

        // Foreign keys
        public string PrescriptionId { get; set; } = string.Empty;
        public Prescription Prescription { get; set; } = null!;
        public string CustomerId { get; set; } = string.Empty;
        public Customer Customer { get; set; } = null!;
    }
}
