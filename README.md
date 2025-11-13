# Ibhayi Pharmacy - Prescription Management System

A comprehensive web-based prescription management system built with ASP.NET Core MVC for Ibhayi Pharmacy in Nelson Mandela Bay metro.

## Features

### For Pharmacy Managers
- Manage pharmacy details and information
- Manage medication inventory (add, edit, delete)
- Manage active ingredients and dosage forms
- Manage supplier records
- Manage doctor records
- Manage pharmacist users
- Monitor and manage medication stock levels
- Create and track stock orders
- Generate stock take reports (PDF)

### For Pharmacists
- Load and register prescriptions
- Dispense medications
- Check for customer allergies
- Track prescription repeats
- Generate dispensing reports (PDF)

### For Customers
- Register as a new user
- Upload prescriptions (PDF)
- Request medication orders
- Manage prescription repeats
- View order status
- Generate prescription reports (PDF)
- Update profile and allergies

## System Requirements

- .NET 8.0 SDK or higher
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code with .NET extensions

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd GqeberhaPharmacy
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Configure Database Connection
Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=GqeberhaPharmacyDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

For SQL Server on Linux/Mac, use:
```json
"DefaultConnection": "Server=localhost;Database=GqeberhaPharmacyDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
```

### 4. Configure Email Service (Optional)
Update the email configuration in `appsettings.json`:

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

### 5. Add Migrations and Create Database

Open Package Manager Console in Visual Studio or use the terminal:

```bash
# Add initial migration
dotnet ef migrations add InitialCreate

# Update database with migrations
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## Default Login Credentials

After running migrations, the system creates a default manager account:

- **Email**: manager@ibhayipharmacy.co.za
- **Password**: Manager@123

## Database Schema

### Core Tables
- **AspNetUsers**: Identity users (managers, pharmacists, customers)
- **Pharmacy**: Pharmacy information
- **Pharmacist**: Pharmacist details linked to users
- **Customer**: Customer details linked to users
- **Doctor**: Doctor information
- **Medication**: Medication inventory
- **ActiveIngredient**: Medication ingredients
- **DosageForm**: Medication dosage forms
- **Supplier**: Medication suppliers
- **MedicationIngredient**: Junction table for medications and ingredients
- **Prescription**: Prescriptions loaded into system
- **PrescriptionItem**: Individual items in prescriptions
- **PrescriptionOrder**: Customer orders for prescriptions
- **PrescriptionDispense**: Record of dispensed medications
- **StockOrder**: Medication stock orders
- **StockOrderItem**: Items in stock orders

## Project Structure

```
GqeberhaPharmacy/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── ManagerController.cs
│   ├── PharmacistController.cs
│   ├── CustomerController.cs
│   └── HomeController.cs
├── Models/               # Data Models
├── Views/                # Razor Views
│   ├── Account/
│   ├── Manager/
│   ├── Pharmacist/
│   ├── Customer/
│   ├── Home/
│   └── Shared/
├── Data/                 # Database Context
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Services/            # Business Services
│   ├── EmailService.cs
│   ├── PdfService.cs
│   └── ReportService.cs
├── wwwroot/             # Static Files
├── Program.cs           # Application Startup
└── appsettings.json    # Configuration
```

## Security Features

- Role-based access control (Manager, Pharmacist, Customer)
- Password encryption using ASP.NET Identity
- Password reset via email
- Protected controllers with [Authorize] attributes
- SQL injection prevention with parameterized queries
- CSRF token protection

## Technologies Used

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **PDF Generation**: QuestPDF
- **Email**: MailKit
- **Frontend**: Bootstrap 5, jQuery
- **Icons**: Font Awesome 6

---

**Version**: 1.0  
**Last Updated**: November 2025