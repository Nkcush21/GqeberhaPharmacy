namespace GqeberhaPharmacy.Models
{
    public class Medication
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public int Schedule { get; set; } // 0-6
        public decimal SalesPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int QuantityOnHand { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Foreign keys
        public string DosageFormId { get; set; } = string.Empty;
        public DosageForm DosageForm { get; set; } = null!;
        public string SupplierId { get; set; } = string.Empty;
        public Supplier Supplier { get; set; } = null!;
        public string PharmacyId { get; set; } = string.Empty;
        public Pharmacy Pharmacy { get; set; } = null!;

        // Navigation properties
        public ICollection<MedicationIngredient> MedicationIngredients { get; set; } = new List<MedicationIngredient>();
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
