# System Architecture

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         Frontend (React)                         │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │  TrackingPage    │  │  ZahlungsSeite   │  │  Home Page    │ │
│  │  - Timeline      │  │  - Method Select │  │               │ │
│  │  - QR Code       │  │  - Stripe Form   │  │               │ │
│  │  - SignalR       │  │  - PayPal Button │  │               │ │
│  └──────────────────┘  └──────────────────┘  └───────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              ↑ ↓ (HTTP/SignalR)
┌─────────────────────────────────────────────────────────────────┐
│                      API Layer (.NET)                            │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │ TrackingCtrl     │  │ ZahlungenCtrl    │  │ TrackingHub   │ │
│  │ (Public)         │  │ (Auth)           │  │ (SignalR)     │ │
│  └──────────────────┘  └──────────────────┘  └───────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              ↑ ↓
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                             │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │ IPaymentService  │  │ ITrackingService │  │ IBenachricht. │ │
│  │                  │  │                  │  │ Service       │ │
│  └──────────────────┘  └──────────────────┘  └───────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              ↑ ↓
┌─────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                           │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │ StripePayment    │  │ TrackingService  │  │ EmailService  │ │
│  │ Service          │  │ - QR Generator   │  │ - MailKit     │ │
│  ├──────────────────┤  └──────────────────┘  └───────────────┘ │
│  │ Ueberweisung     │                                            │
│  │ PaymentService   │  ┌──────────────────────────────────────┐ │
│  └──────────────────┘  │   ApplicationDbContext (EF Core)     │ │
│                        └──────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              ↑ ↓
┌─────────────────────────────────────────────────────────────────┐
│                        Domain Layer                              │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │ Entities:        │  │ Enums:           │  │               │ │
│  │ - Auftrag        │  │ - AuftragStatus  │  │               │ │
│  │ - Rechnung       │  │ - ZahlungsMethode│  │               │ │
│  │ - Zahlung        │  │ - ZahlungsStatus │  │               │ │
│  │ - AuftragTracking│  │                  │  │               │ │
│  └──────────────────┘  └──────────────────┘  └───────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              ↑ ↓
┌─────────────────────────────────────────────────────────────────┐
│                       SQL Server Database                        │
│  Tables: Auftraege, Rechnungen, Zahlungen, AuftragTrackings,    │
│          AuftragStatusHistorien                                  │
└─────────────────────────────────────────────────────────────────┘

                    External Services
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Stripe API  │  │  PayPal API  │  │  SMTP Server │
└──────────────┘  └──────────────┘  └──────────────┘
```

## Data Flow: Tracking Request

```
1. Customer opens tracking link with code & token
   ↓
2. Frontend → GET /api/tracking/{code}?token={token}
   ↓
3. TrackingController validates token
   ↓
4. TrackingService queries database
   ↓
5. Returns TrackingStatusResponse with:
   - Current status
   - Progress percentage
   - History timeline
   - Payment status
   ↓
6. Frontend displays:
   - Progress bar
   - Status timeline
   - QR code
   - Real-time updates via SignalR
```

## Data Flow: Payment Initiation

```
1. Customer selects payment method
   ↓
2. Frontend → POST /api/zahlungen/initiieren
   ↓
3. ZahlungenController → PaymentService
   ↓
4. PaymentService:
   - Stripe: Creates PaymentIntent → returns ClientSecret
   - PayPal: Creates Order → returns ApprovalUrl
   - Überweisung: Generates bank details → returns IBAN/BIC
   ↓
5. Frontend receives payment data:
   - Stripe: Shows Elements form
   - PayPal: Redirects to PayPal
   - Überweisung: Shows bank details + GiroCode QR
   ↓
6. Payment completion:
   - Stripe: Webhook → Update status
   - PayPal: Webhook → Update status
   - Überweisung: Manual confirmation by staff
```

## Security Layers

```
┌─────────────────────────────────────────┐
│ Frontend Security                       │
│ - HTTPS only                            │
│ - Type-safe TypeScript                  │
│ - No sensitive data stored              │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ API Security                            │
│ - CORS configured                       │
│ - Authorization policies                │
│ - Rate limiting (planned)               │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Tracking Security                       │
│ - SHA256 tokens (64 chars)              │
│ - RandomNumberGenerator for codes       │
│ - Masked license plates                 │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Payment Security                        │
│ - No card data stored (PCI DSS)         │
│ - Stripe handles sensitive data         │
│ - Webhook signature verification        │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Database Security                       │
│ - Encrypted connections                 │
│ - No plain text passwords               │
│ - Audit logs for all changes            │
└─────────────────────────────────────────┘
```

## Technology Stack Summary

| Layer          | Technologies                                    |
|----------------|-------------------------------------------------|
| Frontend       | React 18, TypeScript, Vite, SignalR Client     |
| API            | ASP.NET Core 10, SignalR, Swagger              |
| Services       | Stripe.net, PayPalCheckoutSdk, MailKit, QRCoder|
| Data           | Entity Framework Core, SQL Server              |
| Testing        | xUnit, Moq (planned)                           |
| Security       | CodeQL, RandomNumberGenerator, SHA256          |
