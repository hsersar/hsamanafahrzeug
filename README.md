# KFZ-Zulassung Multi-Payment & Live-Tracking System

Ein modernes Fahrzeugzulassungs-System mit Multi-Payment-Integration und QR-Code basiertem Live-Tracking.

## 🚀 Features

### Backend (.NET)
- ✅ **Multi-Payment Support**
  - Stripe (Kreditkarte & SEPA Lastschrift)
  - PayPal
  - Banküberweisung mit GiroCode/EPC QR-Code
- ✅ **Live-Tracking System**
  - QR-Code basierte Auftragsverfolgung
  - Öffentlicher Zugriff ohne Login
  - Real-time Updates über SignalR
- ✅ **Benachrichtigungen**
  - E-Mail Versand (HTML Templates)
  - SMS Support (Twilio Integration vorbereitet)
  - Status-Update Benachrichtigungen
- ✅ **Clean Architecture**
  - Domain Layer (Entities & Enums)
  - Application Layer (Interfaces & DTOs)
  - Infrastructure Layer (Services & Database)
  - API Layer (Controllers & SignalR Hubs)

### Frontend (React + TypeScript)
- ✅ **Tracking Page**
  - Live-Status Anzeige
  - Fortschrittsbalken
  - Status-Timeline
  - QR-Code zum Teilen/Drucken
  - SignalR Real-time Updates
- ✅ **Payment Components**
  - Zahlungsmethoden-Auswahl
  - Stripe Payment Integration vorbereitet
  - PayPal Payment Integration vorbereitet
  - Überweisungsdetails mit GiroCode QR
- ✅ **Mobile-First Design**
  - Responsive Layout
  - Touch-optimierte Bedienung
  - Progressive Web App Ready

## 🛠️ Technologie-Stack

### Backend
- .NET 10
- Entity Framework Core
- SQL Server
- Stripe.net, PayPalCheckoutSdk
- QRCoder, MailKit
- SignalR

### Frontend
- React 18 + TypeScript + Vite
- React Router
- @stripe/react-stripe-js
- @paypal/react-paypal-js
- qrcode.react
- @microsoft/signalr

## 🚀 Installation

### Backend
```bash
dotnet restore
dotnet build
cd src/FahrzeugZulassung.API
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

## 📄 Lizenz

MIT