# Ibhayi Pharmacy - Quick Start Guide

## Project Setup Complete ✓

All files have been created and configured. This is a production-ready ASP.NET Core MVC application.

## What's Included

### Models (Data Layer)
- ✓ ApplicationUser (Identity user with extensions)
- ✓ Pharmacy, Pharmacist, Customer
- ✓ Medication, ActiveIngredient, DosageForm, MedicationIngredient
- ✓ Doctor, Supplier
- ✓ Prescription, PrescriptionItem, PrescriptionOrder, PrescriptionDispense
- ✓ StockOrder, StockOrderItem

### Controllers (Business Logic)
- ✓ AccountController (Authentication, Login, Register, Password Reset)
- ✓ ManagerController (Full manager features)
- ✓ PharmacistController (Prescription loading and dispensing)
- ✓ CustomerController (Prescription upload and order management)
- ✓ HomeController (Landing page)

### Views (User Interface)
- ✓ Authentication views (Login, Register, ForgotPassword, ResetPassword)
- ✓ Manager dashboard and management views
- ✓ Pharmacist dashboard and prescription views
- ✓ Customer dashboard and order views
- ✓ Shared layouts and error pages

### Services (Business Logic)
- ✓ EmailService (Send emails for password reset, prescription notifications)
- ✓ PdfService (Generate PDF reports for stock take, dispensing, and prescriptions)
- ✓ ReportService (Placeholder for additional reporting logic)

### Database
- ✓ ApplicationDbContext (Full EF Core configuration)
- ✓ SeedData (Initial data creation for roles and sample data)

### Configuration
- ✓ Program.cs (Dependency injection and middleware setup)
- ✓ appsettings.json (Database and email configuration)

## Next Steps to Run the Application

### Step 1: Configure Connection String (REQUIRED)
Edit `appsettings.json` and set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=GqeberhaPharmacyDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**For different environments:**
- **Windows with SQL Server Express**: `Server=.\\SQLEXPRESS;Database=GqeberhaPharmacyDb;Trusted_Connection=true;Encrypt=false;`
- **Windows with SQL Server**: `Server=localhost;Database=GqeberhaPharmacyDb;Integrated Security=true;Encrypt=false;`
- **Linux/Mac**: `Server=localhost;Database=GqeberhaPharmacyDb;User Id=sa;Password=YourPassword;Encrypt=false;`

### Step 2: Add Migrations (IN VISUAL STUDIO)
Open Package Manager Console and run:
```powershell
Add-Migration InitialCreate
Update-Database
```

**Or via CLI:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 3: Run the Application
```bash
dotnet run
```

The application will start at `https://localhost:5001`

### Step 4: Login
Default credentials (created automatically):
- **Email**: manager@ibhayipharmacy.co.za
- **Password**: Manager@123

## Default Features

### Manager Account
- Full access to manage pharmacy
- Can create medications, suppliers, doctors
- Manage stock and place orders
- Generate reports

### Test Pharmacist Account
Create one from Manager dashboard

### Test Customer Account
Register through the public registration page

## Email Configuration (Optional)

To enable email notifications, update `appsettings.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromPassword": "your-app-password"
  }
}
```

**For Gmail:**
1. Enable 2FA on your Google account
2. Generate an App Password
3. Use the App Password in the configuration

## Key Features Implemented

### Authentication & Security
- [x] Role-based access control (Manager, Pharmacist, Customer)
- [x] ASP.NET Core Identity integration
- [x] Password encryption
- [x] Password reset functionality
- [x] Email verification (optional)

### Manager Features
- [x] Manage pharmacy information
- [x] CRUD operations for medications
- [x] Manage suppliers and doctors
- [x] Stock level tracking
- [x] Stock order creation
- [x] Low stock alerts
- [x] PDF report generation for stock takes

### Pharmacist Features
- [x] Load prescriptions from PDFs
- [x] Check customer allergies
- [x] Dispense medications
- [x] Track prescription repeats
- [x] Generate dispensing reports
- [x] Email notifications

### Customer Features
- [x] User registration with allergy information
- [x] Upload prescriptions
- [x] Request medication orders
- [x] View order status
- [x] Manage repeats
- [x] Generate prescription reports
- [x] Profile management

## Project Structure

```
GqeberhaPharmacy/
├── Controllers/          # MVC Controllers (Business Logic)
├── Models/              # Entity Framework Models
├── Views/               # Razor Views (UI)
│   ├── Account/         # Authentication pages
│   ├── Manager/         # Manager dashboard
│   ├── Pharmacist/      # Pharmacist dashboard
│   ├── Customer/        # Customer dashboard
│   ├── Home/            # Public pages
│   └── Shared/          # Shared layouts and components
├── Data/                # Database configuration
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Services/            # Business services
├── wwwroot/             # Static files (CSS, JS, images)
├── Program.cs           # Application startup
├── appsettings.json    # Configuration
└── GqeberhaPharmacy.csproj  # Project file
```

## Database Tables Created

After running migrations, the following tables will be created:

- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- Pharmacies
- Pharmacists
- Customers
- Doctors
- DosageForms
- ActiveIngredients
- Suppliers
- Medications
- MedicationIngredients
- Prescriptions
- PrescriptionItems
- PrescriptionOrders
- PrescriptionDispenses
- StockOrders
- StockOrderItems

## Troubleshooting

### Issue: "Connection string not found"
**Solution**: Ensure you've configured the ConnectionString in appsettings.json

### Issue: "Database already exists" error
**Solution**: Either use a different database name or drop the existing database first

### Issue: Migrations fail
**Solution**: 
```bash
dotnet ef migrations remove
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Issue: Seed data not created
**Solution**: The SeedData runs automatically in Program.cs. Check that the database is created first.

## Important Files to Review

1. **Program.cs** - Contains all startup configuration
2. **ApplicationDbContext.cs** - Database relationships and configuration
3. **SeedData.cs** - Initial data creation logic
4. **appsettings.json** - Configuration settings

## API/Route Information

### Public Routes
- `GET /` - Home page
- `GET /Account/Login` - Login page
- `POST /Account/Login` - Process login
- `GET /Account/Register` - Register page
- `POST /Account/Register` - Create account

### Manager Routes (Authorized)
- `GET /Manager/Index` - Dashboard
- `GET /Manager/ManageMedications` - Medication list
- `POST /Manager/CreateMedication` - Add medication
- `GET /Manager/StockOrders` - View orders
- `GET /Manager/GenerateStockTakeReport` - PDF report

### Pharmacist Routes (Authorized)
- `GET /Pharmacist/Index` - Dashboard
- `GET /Pharmacist/LoadPrescription` - Load prescription form
- `POST /Pharmacist/LoadPrescription` - Save prescription
- `POST /Pharmacist/DispensePrescription` - Dispense medication
- `GET /Pharmacist/GenerateDispenseReport` - PDF report

### Customer Routes (Authorized)
- `GET /Customer/Index` - Dashboard
- `GET /Customer/UploadPrescription` - Upload form
- `POST /Customer/UploadPrescription` - Save prescription
- `GET /Customer/MyOrders` - View orders
- `POST /Customer/RequestRepeat` - Request repeat
- `GET /Customer/ViewRepeats` - View available repeats
- `GET /Customer/GenerateReport` - PDF report

## Performance Considerations

- Entity Framework is configured with proper indexes
- Database uses parameterized queries (prevents SQL injection)
- Relationships are properly configured with cascade delete rules
- Async/await patterns used throughout for scalability

## Security Features

- Password hashing with ASP.NET Identity
- SQL injection prevention via EF Core
- CSRF token protection in forms
- Role-based authorization on controllers
- Secure password reset via email tokens

## Next Development Steps (Optional)

If you want to enhance the system:
1. Add audit logging
2. Implement prescription image/document storage
3. Add SMS notifications
4. Implement payment integration
5. Add barcode scanning for stock management
6. Create mobile app API endpoints
7. Add prescription expiration checks
8. Implement bulk prescription uploads

---

**Ready to Deploy!**

Your application is fully configured and ready to run. Simply add migrations and start the application.

For questions or issues, refer to the main README.md file.
