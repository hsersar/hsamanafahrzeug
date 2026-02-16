# 🚗 HSA Mana Fahrzeug - Provisions- und Abrechnungssystem

## 🚀 Schnellstart

**➡️ [Projekt lokal starten (Deutsch)](STARTEN.md)**

**➡️ [Visual Start Guide (Deutsch)](VISUELLER_START.md)**

**➡️ [Quick Start (English)](QUICK_START.md)**

---

## Was ist das?

Ein umfassendes **Provisions- und Abrechnungssystem** für eine Multi-Standort-Fahrzeugzulassungsplattform.

### Hauptfunktionen

- 📊 **Provisionsmodell-Verwaltung** - Standard und standort-spezifische Provisionen
- 💰 **Automatische Monatsabrechnungen** - Grundgebühr + prozentuale Provision
- 🎯 **Plattform-Dashboard** - KPIs und Umsatzübersicht in Echtzeit
- 📈 **Multi-Standort-Support** - Verwaltung mehrerer Franchise-Standorte

---

## 🏗️ Technologie-Stack

### Backend
- **.NET 10** - ASP.NET Core Web API
- **Entity Framework Core** - In-Memory Database (Demo)
- **Clean Architecture** - Domain, Application, Infrastructure, API
- **FluentValidation** - Input-Validierung

### Frontend
- **React 19** - UI Framework
- **TypeScript** - Type Safety
- **Vite** - Build Tool
- **Native Fetch** - API Integration

---

## 📦 Projekt-Struktur

```
hsamanafahrzeug/
├── backend/                    # .NET Backend
│   ├── src/
│   │   ├── FahrzeugZulassung.Domain/
│   │   ├── FahrzeugZulassung.Application/
│   │   ├── FahrzeugZulassung.Infrastructure/
│   │   └── FahrzeugZulassung.API/
│   └── README.md
│
├── frontend/                   # React Frontend
│   ├── src/
│   │   ├── pages/
│   │   ├── services/
│   │   └── types/
│   └── README.md
│
├── STARTEN.md                 # 🇩🇪 Start-Anleitung
├── VISUELLER_START.md         # 🎨 Visueller Guide
├── QUICK_START.md             # 🇬🇧 Quick Start
└── IMPLEMENTATION_SUMMARY.md  # Technische Details
```

---

## 🎯 Geschäftsmodell

```
Monatsabrechnung = Grundgebühr (€199) + Provision (2% vom Umsatz) + MwSt (19%)
```

**Beispiel:**
- Grundgebühr: 199,00 €
- Monatsumsatz: 5.000 €
- Provision (2%): 100,00 €
- **Netto**: 299,00 €
- **Brutto** (mit 19% MwSt): 355,81 €

---

## 📚 Dokumentation

- **[Start-Anleitung (DE)](STARTEN.md)** - Schritt-für-Schritt zum lokalen Start
- **[Visueller Guide (DE)](VISUELLER_START.md)** - Mit Diagrammen und Screenshots
- **[Backend README](backend/README.md)** - API-Dokumentation
- **[Frontend README](frontend/README.md)** - Frontend-Architektur
- **[Implementation Summary](IMPLEMENTATION_SUMMARY.md)** - Technische Details
- **[Quick Start (EN)](QUICK_START.md)** - English instructions

---

## 🔑 Hauptfunktionen im Detail

### 1. Standard-Provisionsmodell
- Grundgebühr: 199,00 €
- Provisionssatz: 2,0%
- Gilt für alle neuen Standorte
- SuperAdmin kann ändern

### 2. Standort-spezifische Provisionen
- Individuelle Anpassungen möglich
- Historie aller Änderungen
- Vorschau-Berechnung

### 3. Monatsabrechnungen
- Automatische Generierung
- Status-Tracking (Entwurf → Erstellt → Versandt → Bezahlt)
- PDF-Export (geplant)
- CSV-Export

### 4. Plattform-Dashboard
- Gesamt-Umsatz aller Standorte
- Aktive Standorte
- Plattform-Einnahmen
- Detaillierte Aufschlüsselung

---

## 🎮 Demo-Daten

Nach dem Start sind folgende Test-Daten vorhanden:

### Standorte
1. **Standort Berlin** - Berliner Str. 1, 10115 Berlin
2. **Standort München** - Münchner Str. 1, 80331 München
3. **Standort Hamburg** - Hamburger Str. 1, 20095 Hamburg

### Standard-Provisionsmodell
- Grundgebühr: €199.00
- Provisionssatz: 2.0%

---

## 🔧 Entwicklung

### Backend starten
```bash
cd backend/src/FahrzeugZulassung.API
dotnet run
```
→ http://localhost:5000

### Frontend starten
```bash
cd frontend
npm install
npm run dev
```
→ http://localhost:3000

---

## 📊 API Endpoints

**Provisionen:**
- `GET/PUT /api/provisionen/standard`
- `GET/PUT /api/provisionen/standort/{id}`

**Abrechnungen:**
- `POST /api/monatsabrechnungen/erstellen`
- `POST /api/monatsabrechnungen/alle-erstellen`
- `GET /api/monatsabrechnungen`
- `GET /api/monatsabrechnungen/plattform-umsatz`

Vollständige API-Dokumentation: [backend/README.md](backend/README.md)

---

## 🎓 Clean Architecture

Das Projekt folgt **Clean Architecture** Prinzipien:

```
┌─────────────────────────────────┐
│   API Controllers (Web)         │
│   - ProvisionsController        │
│   - MonatsabrechnungenController│
└───────────────┬─────────────────┘
                │
┌───────────────▼─────────────────┐
│   Application Services          │
│   - IProvisionsService          │
│   - IMonatsabrechnungService    │
└───────────────┬─────────────────┘
                │
┌───────────────▼─────────────────┐
│   Domain (Business Logic)       │
│   - Entities & Business Rules   │
└───────────────┬─────────────────┘
                │
┌───────────────▼─────────────────┐
│   Infrastructure                │
│   - EF Core & Data Access       │
└─────────────────────────────────┘
```

---

## ✨ Status

- ✅ Backend: Komplett implementiert (19 API Endpoints)
- ✅ Frontend: Basis-Dashboard funktionsfähig
- ✅ Dokumentation: Umfassend (4 Guides)
- ⏳ Authentifizierung: Geplant
- ⏳ Erweiterte UI: Geplant
- ⏳ PDF-Export: Geplant

---

## 🤝 Beitragen

Dieses Projekt ist eine Demo-Implementierung eines Provisions- und Abrechnungssystems.

---

## 📝 Lizenz

Demo-Projekt für Entwicklungszwecke.

---

## 🆘 Hilfe & Support

- **Start-Problem?** → Siehe [STARTEN.md](STARTEN.md)
- **API-Fragen?** → Siehe [backend/README.md](backend/README.md)
- **Frontend-Fragen?** → Siehe [frontend/README.md](frontend/README.md)
- **Technische Details?** → Siehe [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)

---

**Los geht's! → [STARTEN.md](STARTEN.md)** 🚀