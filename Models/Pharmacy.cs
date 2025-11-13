namespace GqeberhaPharmacy.Models
{
    public class Pharmacy
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string HealthCouncilRegistrationNumber { get; set; } = string.Empty;
        public string PhysicalAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Foreign key
        public string? ResponsiblePharmacistId { get; set; }
        public Pharmacist? ResponsiblePharmacist { get; set; }

        // Navigation properties
        public ICollection<Pharmacist> Pharmacists { get; set; } = new List<Pharmacist>();
        public ICollection<ActiveIngredient> ActiveIngredients { get; set; } = new List<ActiveIngredient>();
        public ICollection<DosageForm> DosageForms { get; set; } = new List<DosageForm>();
        public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<StockOrder> StockOrders { get; set; } = new List<StockOrder>();
    }
}
