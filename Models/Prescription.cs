namespace GqeberhaPharmacy.Models
{
    public class Prescription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime PrescriptionDate { get; set; }
        public string? PdfPath { get; set; }
        public int Status { get; set; } = 0; // 0: Pending, 1: Approved, 2: Dispensed, 3: Cancelled
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DispensedDate { get; set; }

        // Foreign keys
        public string CustomerId { get; set; } = string.Empty;
        public Customer Customer { get; set; } = null!;
        public string DoctorId { get; set; } = string.Empty;
        public Doctor Doctor { get; set; } = null!;
        public string PharmacistId { get; set; } = string.Empty;
        public Pharmacist Pharmacist { get; set; } = null!;
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
        public ICollection<PrescriptionOrder> Orders { get; set; } = new List<PrescriptionOrder>();
        public ICollection<PrescriptionDispense> Dispenses { get; set; } = new List<PrescriptionDispense>();
    }
}
