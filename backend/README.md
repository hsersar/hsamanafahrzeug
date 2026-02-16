# Fahrzeug Zulassungs-Management System

## Provisions- und Abrechnungsmodell

Dieses System implementiert ein umfassendes Provisions- und Abrechnungssystem zwischen SuperAdmin (Plattform-Betreiber) und Standorten (Franchise-Nehmer/Partner).

## 🏗️ Architektur

Das Projekt folgt der **Clean Architecture** Struktur:

```
backend/
├── src/
│   ├── FahrzeugZulassung.Domain/          # Domain-Entitäten und Business-Logik
│   │   ├── Entities/
│   │   │   ├── Standort.cs
│   │   │   ├── ProvisionsModell.cs
│   │   │   ├── ProvisionsStaffel.cs
│   │   │   ├── Monatsabrechnung.cs
│   │   │   └── MonatsabrechnungPosition.cs
│   │   └── Enums/
│   │       ├── BenutzerRolle.cs
│   │       ├── MonatsabrechnungStatus.cs
│   │       └── MonatsabrechnungPositionTyp.cs
│   │
│   ├── FahrzeugZulassung.Application/     # Anwendungslogik und DTOs
│   │   ├── Interfaces/
│   │   │   ├── IProvisionsService.cs
│   │   │   └── IMonatsabrechnungService.cs
│   │   ├── DTOs/
│   │   │   └── Provision/
│   │   │       ├── ProvisionsModellDto.cs
│   │   │       ├── MonatsabrechnungDto.cs
│   │   │       ├── PlattformUmsatzDto.cs
│   │   │       └── ...
│   │   └── Validators/
│   │       └── ProvisionsModellUpdateValidator.cs
│   │
│   ├── FahrzeugZulassung.Infrastructure/  # Datenzugriff und externe Services
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   │   ├── ProvisionsModellConfiguration.cs
│   │   │   │   ├── MonatsabrechnungConfiguration.cs
│   │   │   │   └── ...
│   │   │   └── Data/
│   │   │       └── ApplicationDbContext.cs
│   │   └── Services/
│   │       ├── ProvisionsService.cs
│   │       └── MonatsabrechnungService.cs
│   │
│   └── FahrzeugZulassung.API/             # Web API und Endpoints
│       ├── Controllers/
│       │   ├── ProvisionsController.cs
│       │   └── MonatsabrechnungenController.cs
│       ├── Program.cs
│       └── appsettings.json
```

## 🎯 Geschäftsmodell

### Abrechnungsformel

```
Monatsabrechnung an Standort =
    Feste monatliche Grundgebühr (z.B. 199,00 €)
  + Prozentuale Provision auf JEDE Kunden-Rechnung des Monats
    (Standard: 2%, individuell anpassbar pro Standort)
────────────────────────────────────────────────────────
= Gesamtbetrag (Netto) + 19% MwSt = Brutto
```

### Beispiel-Berechnung

- **Grundgebühr**: 199,00 €
- **Standort-Umsatz im Monat**: 5.000 €
- **Anzahl Rechnungen**: 50
- **Provision**: 2% von 5.000 € = 100,00 €
- **Nettobetrag**: 199,00 € + 100,00 € = **299,00 €**
- **Bruttobetrag** (19% MwSt): **355,81 €**

## 🔑 Rollen & Berechtigungen

| Aktion | SuperAdmin | StandortAdmin |
|--------|-----------|---------------|
| Standard-Provisionssatz definieren | ✅ | ❌ |
| Standard-Grundgebühr definieren | ✅ | ❌ |
| Standort-spezifische Provision anpassen | ✅ | ✅ (nur eigener Standort) |
| Alle Transaktionen & Anträge sehen | ✅ (read-only!) | Nur eigener Standort |
| Transaktionen/Anträge ändern | ❌ NEIN! | Nur eigener Standort |
| Monatsabrechnungen erstellen | ✅ | ❌ |
| Monatsabrechnungen einsehen | ✅ (alle) | ✅ (nur eigene) |
| Abrechnungen bezahlen | ❌ | ✅ |

## 📡 API Endpoints

### Provisionsmodell-Verwaltung

#### Standard-Provisionsmodell
```http
GET    /api/provisionen/standard
PUT    /api/provisionen/standard
```

#### Standort-spezifische Provisionen
```http
GET    /api/provisionen/standort/{standortId}
PUT    /api/provisionen/standort/{standortId}
DELETE /api/provisionen/standort/{standortId}
GET    /api/provisionen/standort/{standortId}/historie
GET    /api/provisionen/standort/{standortId}/vorschau?jahr=2026&monat=2
```

### Monatsabrechnungen

#### Abrechnungen erstellen
```http
POST   /api/monatsabrechnungen/erstellen
POST   /api/monatsabrechnungen/alle-erstellen?jahr=2026&monat=2
```

#### Abrechnungen abrufen
```http
GET    /api/monatsabrechnungen
GET    /api/monatsabrechnungen/{id}
GET    /api/monatsabrechnungen/standort/{standortId}?jahr=2026
```

#### Abrechnungs-Aktionen
```http
POST   /api/monatsabrechnungen/{id}/versenden
POST   /api/monatsabrechnungen/{id}/bezahlt
POST   /api/monatsabrechnungen/{id}/stornieren
GET    /api/monatsabrechnungen/{id}/pdf
GET    /api/monatsabrechnungen/export/csv
```

#### Dashboard
```http
GET    /api/monatsabrechnungen/plattform-umsatz?jahr=2026&monat=2
```

## 🚀 Schnellstart

### Backend starten

```bash
cd backend/src/FahrzeugZulassung.API
dotnet run
```

Die API läuft auf: `http://localhost:5000`

### Beispiel-Requests

#### 1. Standard-Provisionsmodell abrufen
```bash
curl http://localhost:5000/api/provisionen/standard
```

**Response:**
```json
{
  "id": "4cc0d161-76b7-41c2-81a4-b8a6e6f16c79",
  "standortId": null,
  "istStandard": true,
  "monatlicheGrundgebuehr": 199.00,
  "provisionsProzentsatz": 2.0,
  "gueltigAb": "2026-02-16T21:11:10.9724128Z",
  "istAktiv": true
}
```

#### 2. Alle Monatsabrechnungen für Februar 2026 erstellen
```bash
curl -X POST "http://localhost:5000/api/monatsabrechnungen/alle-erstellen?jahr=2026&monat=2"
```

**Response:**
```json
[
  {
    "id": "8a7359b2-40f0-4084-ae00-33a2cc46b702",
    "standortId": "48dfe631-4f1d-4962-b6be-129a2cc97fb5",
    "standortName": "Standort Berlin",
    "abrechnungsNummer": "ABR-2026-02-001",
    "jahr": 2026,
    "monat": 2,
    "zeitraum": "Februar 2026",
    "grundgebuehr": 199.00,
    "provisionsProzentsatz": 2.0,
    "nettobetrag": 199.00,
    "steuerbetrag": 37.81,
    "bruttobetrag": 236.81,
    "status": 1,
    "faelligkeitsdatum": "2026-03-02T21:11:18.7052286Z"
  },
  ...
]
```

#### 3. Provisionsmodell für einen Standort aktualisieren
```bash
curl -X PUT http://localhost:5000/api/provisionen/standort/48dfe631-4f1d-4962-b6be-129a2cc97fb5 \
  -H "Content-Type: application/json" \
  -d '{
    "monatlicheGrundgebuehr": 249.00,
    "provisionsProzentsatz": 2.5,
    "aenderungsgrund": "Individuelle Vereinbarung für Premium-Standort"
  }'
```

## 📊 Datenbankschema

### Hauptentitäten

#### ProvisionsModell
- `Id` (Guid)
- `IstStandard` (bool) - Kennzeichnet globales Standard-Modell
- `StandortId` (Guid?) - Null für Standard-Modell
- `MonatlicheGrundgebuehr` (decimal) - z.B. 199,00 €
- `ProvisionsProzentsatz` (decimal) - z.B. 2,0%
- `GueltigAb`, `GueltigBis` (DateTime)
- `IstAktiv` (bool)

#### Monatsabrechnung
- `Id` (Guid)
- `StandortId` (Guid)
- `Jahr`, `Monat` (int)
- `AbrechnungsNummer` (string) - "ABR-2026-02-001"
- `AnzahlRechnungen`, `AnzahlAuftraege` (int)
- `GesamtUmsatz`, `Grundgebuehr`, `ProvisionsBetrag` (decimal)
- `Nettobetrag`, `Steuerbetrag`, `Bruttobetrag` (decimal)
- `Status` (enum) - Entwurf, Erstellt, Versandt, Bezahlt, Überfällig, Storniert
- `Faelligkeitsdatum`, `BezahltAm` (DateTime?)

#### MonatsabrechnungPosition
- `Id` (Guid)
- `MonatsabrechnungId` (Guid)
- `Position` (int)
- `Typ` (enum) - Grundgebühr, Provision, Rabatt, Sonstiges
- `Beschreibung` (string)
- `Menge`, `Einzelpreis`, `Nettobetrag`, etc. (decimal)

## 🔧 Konfiguration

### appsettings.json

```json
{
  "Provision": {
    "StandardGrundgebuehr": 199.00,
    "StandardProzentsatz": 2.0,
    "Zahlungsziel_Tage": 14,
    "AbrechnungsNummerPrefix": "ABR"
  }
}
```

## 🧪 Demo-Daten

Das System wird mit folgenden Demo-Daten gestartet:

### Standorte
1. **Standort Berlin** - Berliner Str. 1, 10115 Berlin
2. **Standort München** - Münchner Str. 1, 80331 München
3. **Standort Hamburg** - Hamburger Str. 1, 20095 Hamburg

### Standard-Provisionsmodell
- Grundgebühr: **199,00 €**
- Provisionssatz: **2,0%**
- Aktiv seit: System-Start

## 📝 Nächste Schritte

### Noch zu implementieren:

1. **Frontend** (React/TypeScript)
   - Provisionsmodell-Verwaltung für SuperAdmin
   - Monatsabrechnungen-Dashboard
   - Plattform-Umsatz-Übersicht
   - StandortAdmin-Ansichten

2. **Authentifizierung & Autorisierung**
   - JWT-Token-basierte Auth
   - Policy-basierte Autorisierung
   - Rollen-Management

3. **Erweiterte Features**
   - ZUGFeRD PDF-Generierung
   - E-Mail-Versand von Abrechnungen
   - Excel/CSV-Export
   - Provisionen-Staffelung
   - Überfällige Abrechnungen-Monitoring

4. **Testing**
   - Unit Tests
   - Integration Tests
   - E2E Tests

## 📚 Technologie-Stack

### Backend
- **.NET 10** - Runtime
- **ASP.NET Core** - Web Framework
- **Entity Framework Core** - ORM
- **FluentValidation** - Validierung
- **In-Memory Database** - Für Demo (produktiv: SQL Server)

### Geplantes Frontend
- **React** - UI Framework
- **TypeScript** - Typsicherheit
- **Vite** - Build Tool
- **TailwindCSS** - Styling

## 📄 Lizenz

Dieses Projekt ist ein Demonstrationsprojekt für ein Provisions- und Abrechnungssystem.

## 👥 Kontakt

Für Fragen und Support kontaktieren Sie bitte das Entwicklungsteam.
