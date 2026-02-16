# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frontend (React + TypeScript)             │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌───────────────┐  ┌──────────────┐  ┌────────────────┐       │
│  │ Notifications │  │     eVB      │  │    Reports     │       │
│  │  Components   │  │  Components  │  │  Components    │       │
│  └───────┬───────┘  └──────┬───────┘  └────────┬───────┘       │
│          │                  │                    │               │
│  ┌───────▼──────────────────▼────────────────────▼───────┐     │
│  │              Custom Hooks & Services                   │     │
│  │  • usePushNotifications  • EVB Validation             │     │
│  └────────────────────────┬───────────────────────────────┘     │
│                           │                                      │
│  ┌────────────────────────▼───────────────────────────────┐    │
│  │            Service Worker (PWA)                         │    │
│  │  • Push Event Handling  • Offline Caching              │    │
│  └─────────────────────────────────────────────────────────┘    │
│                           │                                      │
└───────────────────────────┼──────────────────────────────────────┘
                            │ HTTP/REST API
                            │
┌───────────────────────────▼──────────────────────────────────────┐
│                  API Layer (ASP.NET Core)                        │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌────────────┐  ┌──────────┐  ┌──────────────┐                │
│  │   Push     │  │   EVB    │  │   Reports    │                │
│  │ Controller │  │Controller│  │  Controller  │                │
│  └─────┬──────┘  └────┬─────┘  └──────┬───────┘                │
│        │              │                │                         │
└────────┼──────────────┼────────────────┼─────────────────────────┘
         │              │                │
┌────────▼──────────────▼────────────────▼─────────────────────────┐
│              Application Layer (Interfaces & DTOs)               │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────┐  ┌─────────────────┐  ┌──────────────┐   │
│  │ IPushNotification│  │   IEVBService   │  │IReportService│   │
│  │     Service      │  │                 │  │              │   │
│  └──────────────────┘  └─────────────────┘  └──────────────┘   │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │               IEmailTemplateService                       │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  DTOs: Email Models, Push, Reports, EVB                  │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                   │
└────────┬──────────────┬────────────────┬───────────────┬─────────┘
         │              │                │               │
┌────────▼──────────────▼────────────────▼───────────────▼─────────┐
│           Infrastructure Layer (Implementations)                 │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  Services:                                                        │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ • RazorEmailTemplateService (RazorLight)                 │   │
│  │ • WebPushNotificationService (WebPush)                   │   │
│  │ • EVBService (Validation Logic)                          │   │
│  │ • CsvExportService (CsvHelper)                           │   │
│  │ • PdfReportService (QuestPDF)                            │   │
│  │ • ReportService (Aggregator)                             │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                   │
│  Email Templates:                                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ _Layout.cshtml (Master Template)                         │   │
│  │ • WillkommenEmail  • AuftragErstellt  • StatusUpdate    │   │
│  │ • RechnungErstellt • ZahlungsBestaetigung               │   │
│  │ • AuftragAbgeschlossen • DokumenteAngefordert           │   │
│  │ • PasswortZuruecksetzen • MitarbeiterAktiviert          │   │
│  │ • TrackingInfo                                           │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                   │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│                    Domain Layer (Entities & Enums)               │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  Entities:                        Enums:                         │
│  ┌─────────────────┐             ┌─────────────────┐            │
│  │ • Benutzer      │             │ • AuftragStatus │            │
│  │ • Auftrag       │             │ • AuftragTyp    │            │
│  │ • PushSubscription│           │ • EVBStatus     │            │
│  │ • EVBNummer     │             │ • EVBVerwendungszweck│       │
│  └─────────────────┘             └─────────────────┘            │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

## Data Flow Examples

### 1. Email Template Rendering
```
Controller → IEmailTemplateService → RazorEmailTemplateService
                                    → Load Template from EmailTemplates/
                                    → Render with Model
                                    → Return HTML String
```

### 2. Push Notification Flow
```
Frontend (usePushNotifications)
    ↓ Register Service Worker
    ↓ Get VAPID Public Key (GET /api/push/vapid-public-key)
    ↓ Subscribe to Push Manager
    ↓ Send Subscription (POST /api/push/subscribe)
    ↓
Backend (PushController)
    ↓ Store Subscription in DB
    ↓ Later: Send Push via WebPushNotificationService
    ↓
Service Worker
    ↓ Receive Push Event
    ↓ Show Notification
```

### 3. eVB Validation Flow
```
Frontend (EVBNummerInput)
    ↓ User enters eVB number
    ↓ Format validation (XXX-XXXX)
    ↓ POST /api/evb/validieren
    ↓
Backend (EVBController)
    ↓ IEVBService.ValidiereEVBNummerAsync()
    ↓ Check format (7 chars, no I/O/Q)
    ↓ (Future: Query GDV API)
    ↓ Return validation result
    ↓
Frontend
    ↓ Show visual feedback (✓ or ✗)
```

### 4. Report Export Flow
```
Frontend
    ↓ User requests report
    ↓ GET /api/reports/auftraege/csv
    ↓
Backend (ReportsController)
    ↓ IReportService.GetAuftragsReportAsync(filter)
    ↓ Query data from database
    ↓ IReportService.ExportAlsCsvAsync(data)
    ↓ CsvExportService formats with German settings
    ↓ Return CSV file
    ↓
Browser
    ↓ Download CSV file
```

## Technology Stack Details

### Backend Dependencies
```
FahrzeugZulassung.Infrastructure.csproj:
├── MailKit (Latest)
├── MimeKit (Latest)
├── RazorLight (Latest)
├── WebPush (Latest)
├── CsvHelper (Latest)
└── QuestPDF (Latest)
```

### Frontend Dependencies
```
package.json:
├── react: ^18.3.1
├── react-dom: ^18.3.1
├── react-router-dom: ^6.26.0
├── recharts: ^2.12.7
└── web-push: ^3.6.7
```

## API Endpoints Summary

### Push Notifications
- `POST /api/push/subscribe` - Subscribe to push
- `POST /api/push/unsubscribe` - Unsubscribe from push
- `GET /api/push/vapid-public-key` - Get public VAPID key

### eVB Integration
- `POST /api/evb/validieren` - Validate eVB number
- `GET /api/evb/versicherer` - List insurance companies

### Reports
- `GET /api/reports/auftraege` - Orders report (JSON)
- `GET /api/reports/auftraege/csv` - Orders report (CSV)
- `GET /api/reports/auftraege/pdf` - Orders report (PDF)
- `GET /api/reports/umsatz` - Revenue report
- `GET /api/reports/mitarbeiter` - Employee report
- `GET /api/reports/kunden` - Customer report
- `GET /api/reports/zahlungen` - Payment report
- `GET /api/reports/standorte` - Location report (SuperAdmin)

## Clean Architecture Benefits

1. **Testability**: Each layer can be tested independently
2. **Maintainability**: Changes in one layer don't affect others
3. **Flexibility**: Easy to swap implementations (e.g., different email provider)
4. **Scalability**: Clear separation of concerns
5. **Domain-Driven**: Business logic in Domain layer, pure and focused

## Security Considerations

- Authorization required for all endpoints (except VAPID public key)
- SuperAdmin policy for sensitive reports
- eVB validation prevents injection attacks
- VAPID keys for secure push notifications
- No sensitive data in client-side code
