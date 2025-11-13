using Microsoft.AspNetCore.Identity;

namespace GqeberhaPharmacy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string? CellphoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool PasswordResetRequired { get; set; } = false;

        // Foreign keys
        public string? ManagerPharmacyId { get; set; }
        public Pharmacy? ManagerPharmacy { get; set; }

        // Navigation properties
        public ICollection<Pharmacy> ManagedPharmacies { get; set; } = new List<Pharmacy>();
        public Pharmacist? Pharmacist { get; set; }
        public Customer? Customer { get; set; }
    }
}
