namespace GqeberhaPharmacy.Models
{
    public class Pharmacist
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string CellphoneNumber { get; set; } = string.Empty;
        public string HealthCouncilRegistrationNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<Prescription> LoadedPrescriptions { get; set; } = new List<Prescription>();
        public ICollection<PrescriptionDispense> Dispenses { get; set; } = new List<PrescriptionDispense>();
        public ICollection<Pharmacy> ResponsiblePharmacies { get; set; } = new List<Pharmacy>();
    }
}
