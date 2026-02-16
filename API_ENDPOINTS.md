# API Endpoints Reference

Complete list of all available REST API endpoints in the Fahrzeugzulassungs-Webapp.

## Base URL
- Development: `https://localhost:7001/api` or `http://localhost:5001/api`
- Docker: `http://localhost:5001/api`

## Authentication Endpoints

### POST /api/auth/register
Register a new user account.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "vorname": "Max",
  "nachname": "Mustermann",
  "telefon": "+49 123 456789"
}
```

**Response:** `200 OK` with TokenResponseDto

---

### POST /api/auth/login
Authenticate and receive JWT tokens.

**Request:**
```json
{
  "email": "admin@fahrzeugzulassung.de",
  "password": "Admin@123456789"
}
```

**Response:** `200 OK`
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "abc123...",
  "expiresIn": 900,
  "userId": "guid",
  "email": "admin@fahrzeugzulassung.de",
  "rolle": "SuperAdmin",
  "vorname": "Admin",
  "nachname": "User"
}
```

---

### POST /api/auth/refresh-token
Refresh an expired access token.

**Request:**
```json
{
  "refreshToken": "abc123...",
  "accessToken": "expired_token..."
}
```

**Response:** `200 OK` with new TokenResponseDto

---

### POST /api/auth/logout
Logout and invalidate tokens.

**Response:** `200 OK`

---

## Standorte (Locations) Endpoints

**Role Required:** SuperAdmin

### GET /api/standorte
Get all locations.

**Response:** `200 OK` with List<StandortResponseDto>

---

### GET /api/standorte/{id}
Get a specific location by ID.

**Response:** `200 OK` with StandortResponseDto

---

### POST /api/standorte
Create a new location.

**Request:**
```json
{
  "name": "Standort München",
  "strasse": "Maximilianstraße",
  "hausnummer": "1",
  "plz": "80331",
  "ort": "München",
  "telefon": "+49 89 123456",
  "email": "muenchen@fahrzeugzulassung.de"
}
```

**Response:** `201 Created` with StandortResponseDto

---

### PUT /api/standorte/{id}
Update an existing location.

**Request:** StandortUpdateDto

**Response:** `200 OK` with StandortResponseDto

---

### DELETE /api/standorte/{id}
Delete a location.

**Response:** `204 No Content`

---

## Mitarbeiter (Employees) Endpoints

**Role Required:** StandortAdmin or SuperAdmin

### GET /api/mitarbeiter
Get all employees.

**Response:** `200 OK` with List<MitarbeiterResponseDto>

---

### GET /api/mitarbeiter/{id}
Get a specific employee.

**Response:** `200 OK` with MitarbeiterResponseDto

---

### POST /api/mitarbeiter
Create a new employee.

**Request:**
```json
{
  "email": "employee@example.com",
  "vorname": "Hans",
  "nachname": "Müller",
  "telefon": "+49 89 987654",
  "standortId": "guid"
}
```

**Response:** `201 Created` with MitarbeiterResponseDto

---

### PUT /api/mitarbeiter/{id}
Update an employee.

**Request:** MitarbeiterUpdateDto

**Response:** `200 OK` with MitarbeiterResponseDto

---

### POST /api/mitarbeiter/{id}/activate
Activate an employee account.

**Response:** `200 OK`

---

### POST /api/mitarbeiter/{id}/deactivate
Deactivate an employee account.

**Response:** `200 OK`

---

## Kunden (Customers) Endpoints

**Role Required:** Mitarbeiter, StandortAdmin, or SuperAdmin

### GET /api/kunden
Get all customers.

**Response:** `200 OK` with List<KundeResponseDto>

---

### GET /api/kunden/{id}
Get a specific customer.

**Response:** `200 OK` with KundeResponseDto

---

### POST /api/kunden
Create a new customer.

**Request:**
```json
{
  "vorname": "Maria",
  "nachname": "Schmidt",
  "strasse": "Hauptstraße",
  "hausnummer": "42",
  "plz": "80333",
  "ort": "München",
  "geburtsdatum": "1990-05-15",
  "telefon": "+49 89 111222",
  "email": "maria@example.com"
}
```

**Response:** `201 Created` with KundeResponseDto

---

### PUT /api/kunden/{id}
Update a customer.

**Request:** KundeUpdateDto

**Response:** `200 OK` with KundeResponseDto

---

### DELETE /api/kunden/{id}
Delete a customer.

**Response:** `204 No Content`

---

## Aufträge (Orders) Endpoints

### GET /api/auftraege
Get all orders (filtered by role and location).

**Response:** `200 OK` with List<AuftragResponseDto>

---

### GET /api/auftraege/{id}
Get a specific order.

**Response:** `200 OK` with AuftragResponseDto

---

### POST /api/auftraege
Create a new vehicle registration order (3-step wizard).

**Request:**
```json
{
  "step1": {
    "typ": "Anmeldung",
    "vorname": "Max",
    "nachname": "Mustermann",
    "strasse": "Musterstraße",
    "hausnummer": "123",
    "plz": "80331",
    "ort": "München",
    "geburtsdatum": "1985-03-20",
    "email": "max@example.com",
    "telefon": "+49 89 123456"
  },
  "step2": {
    "fin": "WBA12345678901234",
    "kennzeichen": "M AB 1234",
    "marke": "BMW",
    "modell": "3er",
    "erstzulassung": "2020-01-15",
    "farbe": "Schwarz",
    "hubraum": 2000,
    "leistung": 150,
    "kraftstoffart": "Benzin"
  },
  "step3": {
    "agbAkzeptiert": true,
    "datenschutzAkzeptiert": true
  }
}
```

**Response:** `201 Created` with AuftragResponseDto

---

### PUT /api/auftraege/{id}/status
Update order status.

**Request:**
```json
{
  "status": "InBearbeitung",
  "bemerkungen": "Dokumente werden geprüft"
}
```

**Response:** `200 OK` with AuftragResponseDto

---

### POST /api/auftraege/{id}/submit-ikfz
Submit order to iKFZ system.

**Response:** `200 OK` with AuftragResponseDto (includes iKfz reference)

---

### GET /api/auftraege/kunde/{kundeId}
Get all orders for a specific customer.

**Response:** `200 OK` with List<AuftragResponseDto>

---

## Rechnungen (Invoices) Endpoints

### GET /api/rechnungen/{id}
Get a specific invoice.

**Response:** `200 OK` with RechnungResponseDto

---

### GET /api/rechnungen/auftrag/{auftragId}
Get all invoices for a specific order.

**Response:** `200 OK` with List<RechnungResponseDto>

---

### POST /api/rechnungen
Create a new invoice.

**Role Required:** Mitarbeiter, StandortAdmin, or SuperAdmin

**Request:**
```json
{
  "auftragId": "guid",
  "betrag": 150.00,
  "faelligkeitsdatum": "2026-03-15",
  "positionen": [
    {
      "beschreibung": "Zulassungsgebühr",
      "menge": 1,
      "einzelpreis": 100.00,
      "gesamt": 100.00
    },
    {
      "beschreibung": "Bearbeitungsgebühr",
      "menge": 1,
      "einzelpreis": 50.00,
      "gesamt": 50.00
    }
  ]
}
```

**Response:** `201 Created` with RechnungResponseDto

---

### POST /api/rechnungen/{id}/bezahlen
Mark invoice as paid.

**Request:**
```json
{
  "zahlungsmethode": "Überweisung",
  "zahlungsReferenz": "TXN-12345"
}
```

**Response:** `200 OK` with RechnungResponseDto

---

## Dokumente (Documents) Endpoints

### POST /api/dokumente/upload
Upload a document (multipart/form-data).

**Form Data:**
- `file`: File to upload
- `auftragId`: Order GUID
- `typ`: Document type (Personalausweis, Fahrzeugschein, Unterschrift, Sonstiges)

**Response:** `200 OK` with document metadata

---

### GET /api/dokumente/{id}
Download a document.

**Response:** `200 OK` with file content

---

### DELETE /api/dokumente/{id}
Delete a document.

**Response:** `204 No Content`

---

## Dashboard Endpoint

### GET /api/dashboard
Get dashboard data with KPIs and statistics.

**Response:** `200 OK`
```json
{
  "kpis": [
    {
      "label": "Offene Aufträge",
      "value": "15",
      "trend": "+5%"
    },
    {
      "label": "In Bearbeitung",
      "value": "8",
      "trend": "+2%"
    },
    {
      "label": "Abgeschlossen (Monat)",
      "value": "42",
      "trend": "+15%"
    },
    {
      "label": "Monatsumsatz",
      "value": "€ 6,300",
      "trend": "+20%"
    }
  ],
  "recentAuftraege": [...],
  "umsatzProMonat": [
    {
      "monat": 1,
      "jahr": 2026,
      "umsatz": 5200.00
    },
    ...
  ]
}
```

---

## Health Check Endpoint

### GET /health
Simple health check endpoint (no authentication required).

**Response:** `200 OK`
```json
{
  "status": "healthy",
  "timestamp": "2026-02-16T20:51:28.000Z"
}
```

---

## Authentication

All endpoints (except `/auth/*` and `/health`) require authentication via JWT Bearer token:

```
Authorization: Bearer {your_access_token}
```

## Error Responses

### 400 Bad Request
```json
{
  "errors": {
    "Email": ["Email is required"],
    "Password": ["Password must be at least 12 characters"]
  }
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "Insufficient permissions"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while processing your request"
}
```

## Rate Limiting

- **Limit**: 100 requests per minute per IP address
- **Response when exceeded**: `429 Too Many Requests`

## CORS

Allowed origins (configurable via environment):
- http://localhost:3000
- http://localhost:5173

## Audit Logging

All POST, PUT, DELETE operations are automatically logged with:
- User ID
- IP address
- User agent
- Action performed
- Entity affected
- Old and new values (where applicable)
