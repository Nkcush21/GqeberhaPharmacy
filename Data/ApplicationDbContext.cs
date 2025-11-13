using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GqeberhaPharmacy.Models;

namespace GqeberhaPharmacy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ActiveIngredient> ActiveIngredients { get; set; }
        public DbSet<DosageForm> DosageForms { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationIngredient> MedicationIngredients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<PrescriptionOrder> PrescriptionOrders { get; set; }
        public DbSet<PrescriptionDispense> PrescriptionDispenses { get; set; }
        public DbSet<StockOrder> StockOrders { get; set; }
        public DbSet<StockOrderItem> StockOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pharmacy configuration
            modelBuilder.Entity<Pharmacy>()
                .HasKey(p => p.Id);

            // Pharmacist configuration
            modelBuilder.Entity<Pharmacist>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Pharmacist>()
                .HasOne(p => p.User)
                .WithOne(u => u.Pharmacist)
                .HasForeignKey<Pharmacist>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Pharmacist>()
                .HasOne(p => p.Pharmacy)
                .WithMany(ph => ph.Pharmacists)
                .HasForeignKey(p => p.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer configuration
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ActiveIngredient configuration
            modelBuilder.Entity<ActiveIngredient>()
                .HasKey(ai => ai.Id);
            modelBuilder.Entity<ActiveIngredient>()
                .HasOne(ai => ai.Pharmacy)
                .WithMany(p => p.ActiveIngredients)
                .HasForeignKey(ai => ai.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // DosageForm configuration
            modelBuilder.Entity<DosageForm>()
                .HasKey(df => df.Id);
            modelBuilder.Entity<DosageForm>()
                .HasOne(df => df.Pharmacy)
                .WithMany(p => p.DosageForms)
                .HasForeignKey(df => df.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Supplier configuration
            modelBuilder.Entity<Supplier>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.Pharmacy)
                .WithMany(p => p.Suppliers)
                .HasForeignKey(s => s.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Medication configuration
            modelBuilder.Entity<Medication>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Medication>()
                .HasIndex(m => new { m.Name, m.PharmacyId })
                .IsUnique();
            modelBuilder.Entity<Medication>()
                .HasOne(m => m.DosageForm)
                .WithMany(df => df.Medications)
                .HasForeignKey(m => m.DosageFormId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Medication>()
                .HasOne(m => m.Supplier)
                .WithMany(s => s.Medications)
                .HasForeignKey(m => m.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Medication>()
                .HasOne(m => m.Pharmacy)
                .WithMany(p => p.Medications)
                .HasForeignKey(m => m.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // MedicationIngredient configuration
            modelBuilder.Entity<MedicationIngredient>()
                .HasKey(mi => mi.Id);
            modelBuilder.Entity<MedicationIngredient>()
                .HasOne(mi => mi.Medication)
                .WithMany(m => m.MedicationIngredients)
                .HasForeignKey(mi => mi.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MedicationIngredient>()
                .HasOne(mi => mi.ActiveIngredient)
                .WithMany(ai => ai.MedicationIngredients)
                .HasForeignKey(mi => mi.ActiveIngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor configuration
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.Id);
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Pharmacy)
                .WithMany(p => p.Doctors)
                .HasForeignKey(d => d.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prescription configuration
            modelBuilder.Entity<Prescription>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Prescriptions)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Pharmacist)
                .WithMany(ph => ph.LoadedPrescriptions)
                .HasForeignKey(p => p.PharmacistId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Pharmacy)
                .WithMany(ph => ph.Medications)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionItem configuration
            modelBuilder.Entity<PrescriptionItem>()
                .HasKey(pi => pi.Id);
            modelBuilder.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Medication)
                .WithMany(m => m.PrescriptionItems)
                .HasForeignKey(pi => pi.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionOrder configuration
            modelBuilder.Entity<PrescriptionOrder>()
                .HasKey(po => po.Id);
            modelBuilder.Entity<PrescriptionOrder>()
                .HasOne(po => po.Prescription)
                .WithMany(p => p.Orders)
                .HasForeignKey(po => po.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PrescriptionOrder>()
                .HasOne(po => po.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(po => po.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionDispense configuration
            modelBuilder.Entity<PrescriptionDispense>()
                .HasKey(pd => pd.Id);
            modelBuilder.Entity<PrescriptionDispense>()
                .HasOne(pd => pd.Pharmacist)
                .WithMany(p => p.Dispenses)
                .HasForeignKey(pd => pd.PharmacistId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PrescriptionDispense>()
                .HasOne(pd => pd.Prescription)
                .WithMany(p => p.Dispenses)
                .HasForeignKey(pd => pd.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockOrder configuration
            modelBuilder.Entity<StockOrder>()
                .HasKey(so => so.Id);
            modelBuilder.Entity<StockOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.StockOrders)
                .HasForeignKey(so => so.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StockOrder>()
                .HasOne(so => so.Pharmacy)
                .WithMany(p => p.StockOrders)
                .HasForeignKey(so => so.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);

            // StockOrderItem configuration
            modelBuilder.Entity<StockOrderItem>()
                .HasKey(soi => soi.Id);
            modelBuilder.Entity<StockOrderItem>()
                .HasOne(soi => soi.StockOrder)
                .WithMany(so => so.Items)
                .HasForeignKey(soi => soi.StockOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StockOrderItem>()
                .HasOne(soi => soi.Medication)
                .WithMany()
                .HasForeignKey(soi => soi.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
