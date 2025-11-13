namespace GqeberhaPharmacy.Models
{
    public class PrescriptionItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Quantity { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int NumberOfRepeats { get; set; }
        public int RepeatsUsed { get; set; } = 0;

        // Foreign keys
        public string PrescriptionId { get; set; } = string.Empty;
        public Prescription Prescription { get; set; } = null!;
        public string MedicationId { get; set; } = string.Empty;
        public Medication Medication { get; set; } = null!;
    }
}
