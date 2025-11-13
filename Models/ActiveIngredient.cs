namespace GqeberhaPharmacy.Models
{
    public class ActiveIngredient
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<MedicationIngredient> MedicationIngredients { get; set; } = new List<MedicationIngredient>();
    }
}
