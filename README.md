# KFZ-Zulassungsservice

Eine moderne Webanwendung für die digitale Fahrzeugzulassung mit professionellen E-Mail-Templates, Push-Benachrichtigungen, eVB-Integration und erweiterten Reportfunktionen.

## Features

### 1. Professionelle HTML E-Mail Templates
- 10 responsive E-Mail-Templates mit Corporate Design
- RazorLight Template-Engine für typsichere Models
- Master-Layout für einheitliches Design
- Inline CSS für maximale E-Mail-Client-Kompatibilität

### 2. PWA Push Notifications
- Web Push API Integration
- VAPID-basierte Push-Benachrichtigungen
- Service Worker für Offline-Funktionalität
- Benachrichtigungen bei Statusänderungen

### 3. eVB-Nummer Integration
- Validierung elektronischer Versicherungsbestätigungen
- Format-Prüfung (7-stellig, alphanumerisch)
- Live-Validierung mit visuellem Feedback
- Vorbereitet für GDV-API-Integration

### 4. Erweiterte Reports
- 6 verschiedene Report-Typen
- CSV-Export mit deutscher Formatierung
- PDF-Generierung mit QuestPDF
- Druckoptimierte Ansichten

## Technologie-Stack

### Backend (.NET 10)
- Clean Architecture (Domain, Application, Infrastructure, API)
- MailKit & MimeKit (E-Mail-Versand)
- RazorLight (Template-Rendering)
- WebPush (Push-Benachrichtigungen)
- CsvHelper (CSV-Export)
- QuestPDF (PDF-Generierung)

### Frontend (React + TypeScript)
- React 18 UI-Framework
- TypeScript Type-Safe Development
- Service Worker PWA-Funktionalität
- Web Push API Push-Benachrichtigungen

## Installation

### Backend
```bash
cd backend
dotnet restore
dotnet build
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

## Konfiguration

Siehe `backend/src/FahrzeugZulassung.API/appsettings.json` für Push Notifications, eVB und Reports Einstellungen.