namespace GqeberhaPharmacy.Models
{
    public class PrescriptionDispense
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DispenseDate { get; set; } = DateTime.Now;
        public int QuantityDispensed { get; set; }

        // Foreign keys
        public string PrescriptionItemId { get; set; } = string.Empty;
        public string PharmacistId { get; set; } = string.Empty;
        public Pharmacist Pharmacist { get; set; } = null!;
        public string PrescriptionId { get; set; } = string.Empty;
        public Prescription Prescription { get; set; } = null!;
    }
}
