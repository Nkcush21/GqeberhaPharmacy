# 🔐 Default Credentials & Testing Guide

## Default Account (Auto-Created)

**Manager Account**
```
Email:    manager@ibhayipharmacy.co.za
Password: Manager@123
```

This account is automatically created when you run the database migrations.

---

## Test Accounts to Create

### 1. Create a Test Pharmacist
As Manager, navigate to: **Manager Dashboard → Manage Pharmacists**

Suggested Test Data:
```
First Name:     John
Last Name:      Smith
ID Number:      9805123456789
Cellphone:      0721234567
Health Council: HC-2024-001
Email:          pharmacist@test.co.za
```

### 2. Create a Test Customer
Use public registration page: **Register**

Suggested Test Data:
```
First Name:     Jane
Last Name:      Doe
Email:          jane.doe@test.co.za (IMPORTANT: Use this to login)
ID Number:      8901234567890
Cellphone:      0731234567
Allergies:      Penicillin, Aspirin
Password:       Test@123456
```

---

## Sample Data Pre-Created

The system automatically creates:

✅ **Default Pharmacy**
- Name: Ibhayi Pharmacy
- Health Council Reg: REG001
- Address: 123 Main Street, Port Elizabeth
- Contact: +27 41 123 4567
- Email: info@ibhayipharmacy.co.za

✅ **Sample Dosage Forms**
- Tablet
- Capsule
- Liquid

✅ **Sample Active Ingredients**
- Paracetamol
- Ibuprofen
- Amoxicillin

✅ **Sample Supplier**
- MediSupply Co
- Contact: Jane Smith
- Email: jane@medisupply.co.za

---

## Testing Workflow

### Test 1: Manager Features
1. Login as manager
2. Go to Dashboard
3. Create new medication:
   - Name: Aspirin 500mg
   - Schedule: 2
   - Price: 12.50
   - Reorder Level: 20
   - Qty on Hand: 100
4. View low stock medications
5. Create a stock order
6. Generate a stock take report (PDF)

### Test 2: Customer Features
1. Register as new customer
2. Fill in allergies
3. Upload a prescription (PDF)
4. View orders
5. Generate prescription report

### Test 3: Pharmacist Features
1. Login as pharmacist (use pharmacist account)
2. Load a prescription:
   - Select patient
   - Select doctor
   - Add items
   - Set repeats
3. View prescription details
4. Dispense medication
5. Check notifications
6. Generate dispensing report

---

## Important Notes for Testing

### Email Notifications
- Default emails won't be sent unless SMTP is configured
- Configure in appsettings.json:
  ```json
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromPassword": "your-app-password"
  }
  ```

### PDF Reports
- All PDF generation is fully functional
- Reports are generated in memory
- Can be downloaded immediately

### Database Reset
If you need to reset everything:
```powershell
# In Package Manager Console
Update-Database 0
Remove-Migration -Force
Remove-Migration -Force
Add-Migration InitialCreate
Update-Database
```

Or via CLI:
```bash
dotnet ef database drop
dotnet ef database update
```

---

## Login URLs

- **Login Page**: `/Account/Login`
- **Register Page**: `/Account/Register`
- **Forgot Password**: `/Account/ForgotPassword`

---

## Dashboard URLs (After Login)

- **Manager Dashboard**: `/Manager/Index`
- **Pharmacist Dashboard**: `/Pharmacist/Index`
- **Customer Dashboard**: `/Customer/Index`

---

## Common Test Scenarios

### Scenario 1: Order Medication
```
1. Customer registers
2. Customer uploads prescription PDF
3. Pharmacist loads prescription
4. Pharmacist dispenses medication
5. Customer receives email notification
6. Customer logs in and views order
```

### Scenario 2: Check Low Stock
```
1. Manager creates medication with Qty: 5, Reorder: 10
2. Go to Low Stock Medications
3. Medication appears in list
4. Create stock order
5. Supplier receives email
6. Mark order as received
```

### Scenario 3: Request Repeat
```
1. Customer uploads prescription with repeats
2. Prescription is dispensed (1st time)
3. Customer requests repeat
4. Pharmacist sees repeat request
5. Dispenses again if repeats available
```

---

## Test Data Template

### Doctor
```
First Name:    Dr.
Last Name:     Johnson
Practice No:   DOC-2024-001
Contact:       0219876543
Email:         dr.johnson@practice.co.za
```

### Supplier
```
Name:           MediSupply Plus
Contact Person: Peter Wilson
Email:          peter@medisupply.co.za
Phone:          0215551234
Address:        42 Medical Ave, Cape Town
```

### Medication
```
Name:           Aspirin 500mg
Schedule:       2
Dosage Form:    Tablet
Supplier:       MediSupply Co
Price:          R15.50
Reorder Level:  50
Qty on Hand:    200
Ingredients:    Acetylsalicylic Acid 500mg
```

---

## Performance Testing Notes

- Database queries are optimized
- Include directives prevent N+1 queries
- Async/await used throughout
- PDF generation is memory-efficient
- Email sending is asynchronous

---

## Troubleshooting Login Issues

| Issue | Solution |
|-------|----------|
| "Invalid login attempt" | Check email and password spelling |
| "User not found" | Ensure account was created |
| "Cannot access Manager Dashboard" | Ensure user has Manager role |
| Password reset not working | Configure SMTP in appsettings.json |
| Email not received | Check email configuration and spam folder |

---

## First Time Setup Checklist

- [ ] Configured SQL Server connection string
- [ ] Ran migrations successfully
- [ ] Application starts without errors
- [ ] Can login as manager
- [ ] Can view manager dashboard
- [ ] Can create medication
- [ ] Can register as customer
- [ ] Can upload prescription
- [ ] Can view orders as customer

---

**Everything is ready to test. Start by logging in with the manager account!**
