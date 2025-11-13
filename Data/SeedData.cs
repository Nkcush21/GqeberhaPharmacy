using Microsoft.AspNetCore.Identity;
using GqeberhaPharmacy.Models;

namespace GqeberhaPharmacy.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create roles
            string[] roles = { "Manager", "Pharmacist", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user
            var adminUser = await userManager.FindByEmailAsync("manager@ibhayipharmacy.co.za");
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "manager@ibhayipharmacy.co.za",
                    Email = "manager@ibhayipharmacy.co.za",
                    FirstName = "John",
                    LastName = "Manager",
                    IdNumber = "1234567890123",
                    CellphoneNumber = "0721234567"
                };

                var result = await userManager.CreateAsync(admin, "Manager@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Manager");
                }
            }

            // Create sample pharmacy
            if (!context.Pharmacies.Any())
            {
                var pharmacy = new Pharmacy
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Ibhayi Pharmacy",
                    HealthCouncilRegistrationNumber = "REG001",
                    PhysicalAddress = "123 Main Street, Port Elizabeth",
                    ContactNumber = "+27 41 123 4567",
                    EmailAddress = "info@ibhayipharmacy.co.za",
                    WebsiteUrl = "https://www.ibhayipharmacy.co.za",
                    CreatedDate = DateTime.Now
                };

                context.Pharmacies.Add(pharmacy);

                // Add sample dosage forms
                var dosageForms = new[]
                {
                    new DosageForm { Id = Guid.NewGuid().ToString(), Name = "Tablet", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now },
                    new DosageForm { Id = Guid.NewGuid().ToString(), Name = "Capsule", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now },
                    new DosageForm { Id = Guid.NewGuid().ToString(), Name = "Liquid", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now }
                };
                context.DosageForms.AddRange(dosageForms);

                // Add sample active ingredients
                var ingredients = new[]
                {
                    new ActiveIngredient { Id = Guid.NewGuid().ToString(), Name = "Paracetamol", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now },
                    new ActiveIngredient { Id = Guid.NewGuid().ToString(), Name = "Ibuprofen", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now },
                    new ActiveIngredient { Id = Guid.NewGuid().ToString(), Name = "Amoxicillin", PharmacyId = pharmacy.Id, CreatedDate = DateTime.Now }
                };
                context.ActiveIngredients.AddRange(ingredients);

                // Add sample suppliers
                var supplier = new Supplier
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "MediSupply Co",
                    ContactPersonName = "Jane Smith",
                    ContactPersonEmail = "jane@medisupply.co.za",
                    ContactPersonPhone = "+27 21 987 6543",
                    PharmacyId = pharmacy.Id,
                    CreatedDate = DateTime.Now
                };
                context.Suppliers.Add(supplier);

                await context.SaveChangesAsync();
            }
        }
    }
}
