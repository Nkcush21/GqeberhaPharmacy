namespace GqeberhaPharmacy.Models
{
    public class Doctor
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PracticeNumber { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
