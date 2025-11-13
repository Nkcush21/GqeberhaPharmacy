namespace GqeberhaPharmacy.Models
{
    public class MedicationIngredient
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Strength { get; set; } = string.Empty;

        // Foreign keys
        public string MedicationId { get; set; } = string.Empty;
        public Medication Medication { get; set; } = null!;
        public string ActiveIngredientId { get; set; } = string.Empty;
        public ActiveIngredient ActiveIngredient { get; set; } = null!;
    }
}
