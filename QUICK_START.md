# ZUGFeRD E-Rechnung Integration - Quick Start Guide

## Getting Started in 5 Minutes

### Prerequisites
- .NET 10 SDK
- Node.js 18+
- Terminal/Command Prompt

### 1. Start the Backend API

```bash
cd src/FahrzeugZulassung.API
dotnet run
```

The API will start on `http://localhost:5000` and create a SQLite database with sample data:
- ✅ 1 Standort (KFZ-Zulassungsstelle München GmbH)
- ✅ 2 Kunden (Anna Schmidt, Michael Weber)

### 2. Start the Frontend

```bash
cd frontend
npm install  # First time only
npm start
```

The React app will open at `http://localhost:3000`

### 3. Create Your First ZUGFeRD Invoice

1. Navigate to the invoices page (opens automatically)
2. Click "Neue Rechnung" button
3. Select a customer (Anna Schmidt or Michael Weber)
4. Select the location (Hauptstelle München)
5. Choose ZUGFeRD Profile (Comfort recommended)
6. Add invoice positions:
   - Beschreibung: "Fahrzeuganmeldung"
   - Menge: 1
   - Einzelpreis: 100.00
   - MwSt: 19%
7. Click "Rechnung erstellen"

### 4. Download ZUGFeRD Files

On the invoice list:
- Click "PDF" button to download the PDF with embedded ZUGFeRD XML
- Click "XML" button to download the standalone ZUGFeRD XML

## API Swagger Documentation

Navigate to `http://localhost:5000/swagger` to see interactive API documentation.

## Sample API Calls

### Create Invoice (cURL)

```bash
curl -X POST http://localhost:5000/api/rechnungen \
  -H "Content-Type: application/json" \
  -d '{
    "kundeId": "YOUR_KUNDE_ID",
    "standortId": "YOUR_STANDORT_ID",
    "format": 4,
    "positionen": [
      {
        "beschreibung": "Fahrzeuganmeldung",
        "menge": 1,
        "einzelpreis": 100.00,
        "steuersatz": 19
      }
    ]
  }'
```

### Get All Invoices

```bash
curl http://localhost:5000/api/rechnungen
```

### Get All Customers

```bash
curl http://localhost:5000/api/kunden
```

## Project Structure Overview

```
/home/runner/work/hsamanafahrzeug/hsamanafahrzeug/
├── src/
│   ├── FahrzeugZulassung.Domain/
│   ├── FahrzeugZulassung.Infrastructure/
│   └── FahrzeugZulassung.API/           # ← Start here
├── frontend/                             # ← Then start this
├── tests/
└── README files
```

## Running Tests

```bash
dotnet test
```

Expected output:
```
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7
```

## ZUGFeRD Profiles Explained

| Profile | Description | Use Case |
|---------|-------------|----------|
| **Minimum** | Basic invoice data | Simple invoicing |
| **Basic WL** | Basic without line items | Summary invoices |
| **Basic** | Standard invoice | General purpose |
| **Comfort** ⭐ | EN16931 compliant | **Recommended** - Legal requirement for B2G |
| **Extended** | Full details | Complex invoicing |

## Common Invoice Items for Vehicle Registration

The system supports typical KFZ-Zulassung services:

1. **Fahrzeuganmeldung** (Vehicle Registration) - €100.00
2. **Fahrzeugabmeldung** (Vehicle Deregistration) - €50.00
3. **Wunschkennzeichen-Reservierung** (Custom Plate Reservation) - €50.00
4. **Kurzzeitkennzeichen** (Temporary Plate) - €30.00
5. **Servicegebühr** (Service Fee) - €25.00

## Tax Rates

- **19%** - Standard VAT (Regelsteuersatz)
- **7%** - Reduced VAT (Ermäßigter Steuersatz)
- **0%** - Tax-free (Steuerfrei)

## Troubleshooting

### Port Already in Use
```bash
# Backend
dotnet run --urls "http://localhost:5001"

# Frontend (edit package.json)
PORT=3001 npm start
```

### Database Issues
Delete `fahrzeugzulassung.db` and restart the API to recreate with fresh seed data.

### Frontend Build Errors
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
npm start
```

## Next Steps

1. **Customize Seed Data**: Edit `src/FahrzeugZulassung.Infrastructure/DbSeeder.cs`
2. **Add More Customers**: Use POST `/api/kunden` endpoint
3. **Add More Locations**: Use POST `/api/standorte` endpoint
4. **Explore the Code**: Start with the controllers in `src/FahrzeugZulassung.API/Controllers/`

## Learn More

- **Full Documentation**: See `PROJEKT_README.md`
- **Implementation Details**: See `IMPLEMENTATION_SUMMARY.md`
- **ZUGFeRD Specification**: https://www.ferd-net.de/

---

**🎉 You're now ready to generate ZUGFeRD-compliant e-invoices!**
