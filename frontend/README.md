# Frontend - Fahrzeugzulassung Provisions- und Abrechnungsverwaltung

## 📝 Übersicht

Dieses ist das Frontend für das Provisions- und Abrechnungssystem. Es bietet Benutzeroberflächen für SuperAdmins und StandortAdmins zur Verwaltung von Provisionsmodellen und Monatsabrechnungen.

## 🚀 Schnellstart

### Installation

```bash
npm install
```

### Entwicklungsserver starten

```bash
npm run dev
```

Die Anwendung läuft auf: `http://localhost:3000`

**Wichtig:** Der Backend-Server muss auf `http://localhost:5000` laufen!

### Build für Produktion

```bash
npm run build
```

### Vorschau der Produktionsversion

```bash
npm run preview
```

## 📁 Projektstruktur

```
frontend/
├── src/
│   ├── pages/
│   │   ├── SuperAdmin/
│   │   │   ├── SuperAdminDashboard.tsx   ✅ Implementiert
│   │   │   ├── ProvisionsVerwaltung.tsx  ⏳ Geplant
│   │   │   ├── MonatsabrechnungenPage.tsx ⏳ Geplant
│   │   │   └── PlattformDashboardPage.tsx ⏳ Geplant
│   │   └── StandortAdmin/
│   │       └── MeineProvision.tsx         ⏳ Geplant
│   │
│   ├── components/
│   │   ├── provision/                     ⏳ Geplant
│   │   │   ├── ProvisionsFormular.tsx
│   │   │   └── ProvisionsVorschau.tsx
│   │   └── abrechnung/                    ⏳ Geplant
│   │       ├── AbrechnungDetail.tsx
│   │       ├── AbrechnungErstellenDialog.tsx
│   │       └── AbrechnungsTabelle.tsx
│   │
│   ├── services/                          ✅ Implementiert
│   │   ├── provisionsApi.ts
│   │   └── monatsabrechnungApi.ts
│   │
│   ├── types/                             ✅ Implementiert
│   │   └── provision.ts
│   │
│   ├── App.tsx                            ✅ Implementiert
│   └── main.tsx                           ✅ Implementiert
│
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── README.md
```

## 🎨 Funktionen

### ✅ Implementiert

#### SuperAdmin Dashboard
- **Standard-Provisionsmodell anzeigen**
  - Grundgebühr und Provisionssatz
  - Gültigkeitsdatum und Status

- **Plattform-Umsatz-Übersicht**
  - Anzahl aktiver Standorte
  - Gesamtumsatz aller Standorte
  - Plattform-Einnahmen
  - Grundgebühren und Provisionen
  - Bezahlte Abrechnungen

- **Monatsabrechnungen-Liste**
  - Abrechnungsnummer
  - Standort-Name
  - Zeitraum
  - Netto- und Bruttobeträge
  - Status-Badge

- **Batch-Aktionen**
  - Alle Abrechnungen für alle Standorte erstellen

#### API-Services
- **provisionsApi.ts**
  - Standard-Modell abrufen und aktualisieren
  - Standort-spezifische Modelle verwalten
  - Historie abrufen
  - Vorschau-Berechnung

- **monatsabrechnungApi.ts**
  - Abrechnungen erstellen (einzeln und batch)
  - Abrechnungen abrufen (alle, nach Standort, nach ID)
  - Aktionen: Versenden, Bezahlt markieren, Stornieren
  - PDF/CSV Export
  - Plattform-Umsatz-Dashboard

#### TypeScript Types
- Vollständige Type-Definitionen für alle DTOs
- Enums für Status und Typen

### ⏳ Geplante Features

1. **ProvisionsVerwaltung** (SuperAdmin)
   - Standard-Provision editieren mit Formular
   - Standort-Übersicht mit individuellen Provisionen
   - Änderungshistorie pro Standort
   - Live-Vorschau für Änderungen

2. **MonatsabrechnungenPage** (SuperAdmin)
   - Erweiterte Filterung und Sortierung
   - Batch-Operationen (Versenden, Stornieren)
   - Detailansicht mit Positionen
   - Export-Funktionen (CSV, PDF)

3. **PlattformDashboardPage** (SuperAdmin)
   - Charts und Visualisierungen
     - Umsatzentwicklung über Zeit
     - Standort-Ranking
     - Provisions-Verteilung
   - KPI-Cards
   - Warnungen für überfällige Abrechnungen

4. **MeineProvision** (StandortAdmin)
   - Eigenes Provisionsmodell anzeigen
   - Eigene Provisionen anpassen
   - Eigene Monatsabrechnungen einsehen
   - PDF-Downloads

5. **Zusätzliche Komponenten**
   - Formular-Komponenten mit Validierung
   - Tabellen mit Sortierung und Paginierung
   - Modal-Dialoge
   - Toast-Notifications
   - Loading-States und Error-Handling

## 🔌 API-Integration

Das Frontend kommuniziert mit dem Backend über REST APIs:

### Base URL
```
http://localhost:5000/api
```

### Endpoints

#### Provisionen
- `GET /provisionen/standard`
- `PUT /provisionen/standard`
- `GET /provisionen/standort/{id}`
- `PUT /provisionen/standort/{id}`
- `DELETE /provisionen/standort/{id}`
- `GET /provisionen/standort/{id}/historie`
- `GET /provisionen/standort/{id}/vorschau?jahr=X&monat=Y`

#### Monatsabrechnungen
- `POST /monatsabrechnungen/erstellen`
- `POST /monatsabrechnungen/alle-erstellen?jahr=X&monat=Y`
- `GET /monatsabrechnungen`
- `GET /monatsabrechnungen/{id}`
- `GET /monatsabrechnungen/standort/{id}`
- `POST /monatsabrechnungen/{id}/versenden`
- `POST /monatsabrechnungen/{id}/bezahlt`
- `POST /monatsabrechnungen/{id}/stornieren`
- `GET /monatsabrechnungen/{id}/pdf`
- `GET /monatsabrechnungen/export/csv`
- `GET /monatsabrechnungen/plattform-umsatz?jahr=X`

## 🛠️ Technologie-Stack

- **React 19** - UI Framework
- **TypeScript** - Type Safety
- **Vite** - Build Tool & Dev Server
- **Native Fetch API** - HTTP Requests

### Geplante Erweiterungen
- **React Router** - Navigation
- **TailwindCSS** - Styling
- **Chart.js / Recharts** - Visualisierungen
- **React Hook Form** - Formular-Verwaltung
- **Zod** - Runtime-Validierung

## 📸 Screenshots

### SuperAdmin Dashboard (Aktuell)

Das Dashboard zeigt:
1. Standard-Provisionsmodell mit Grundgebühr und Provisionssatz
2. Plattform-Umsatz-KPIs (Standorte, Umsatz, Einnahmen)
3. Liste der Monatsabrechnungen mit Status
4. Button zum Erstellen aller Abrechnungen

## 🧪 Testing

### Backend testen
```bash
# Backend muss laufen
cd backend/src/FahrzeugZulassung.API
dotnet run
```

### Frontend testen
```bash
# In einem neuen Terminal
cd frontend
npm run dev
```

Browser öffnen: `http://localhost:3000`

## 🔐 Authentifizierung

**Hinweis:** Die Authentifizierung ist derzeit noch nicht implementiert. 
Alle API-Endpoints sind ohne Authentifizierung erreichbar.

Geplant:
- JWT-basierte Authentifizierung
- Role-based Access Control (RBAC)
- SuperAdmin, StandortAdmin, Mitarbeiter Rollen

## 📝 Entwicklung

### Code-Style
- Functional Components mit Hooks
- TypeScript strict mode
- Prop Types via Interfaces
- Clean Code Principles

### Best Practices
- Error Handling in allen API-Calls
- Loading States für bessere UX
- Responsive Design
- Accessibility (ARIA)

## 🐛 Bekannte Probleme

1. Authentifizierung fehlt noch
2. Keine Fehlerbehandlung für Netzwerkfehler außer Alerts
3. Keine Optimierung für mobile Geräte
4. Charts und Visualisierungen fehlen

## 🚀 Deployment

### Produktions-Build erstellen
```bash
npm run build
```

Dies erstellt optimierte Dateien im `dist/` Ordner.

### Deployment-Optionen
- **Netlify** - Einfaches Static Hosting
- **Vercel** - Optimiert für React
- **AWS S3 + CloudFront** - Skalierbar
- **Docker** - Containerized Deployment

## 📚 Weitere Informationen

Siehe auch:
- Backend README: `../backend/README.md`
- API Dokumentation: Backend OpenAPI/Swagger
- Issue Tracker: GitHub Issues

## 👥 Kontakt

Für Fragen und Support kontaktieren Sie bitte das Entwicklungsteam.
