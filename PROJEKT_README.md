# Fahrzeug Zulassung - ZUGFeRD E-Rechnung Integration

Eine Anwendung zur Fahrzeugzulassung mit integrierter ZUGFeRD-konformer E-Rechnungserstellung.

## Funktionen

- ✅ **ZUGFeRD 2.1.1 konforme E-Rechnungen** (PDF/A-3 mit eingebettetem XML)
- ✅ **Mehrere Profile**: Minimum, Basic, Comfort (EN16931), Extended
- ✅ **Automatische Berechnung** von Netto, MwSt und Brutto
- ✅ **PDF und XML Download** für jede Rechnung
- ✅ **Kunde und Standort Verwaltung**
- ✅ **Flexible Rechnungspositionen** mit verschiedenen Steuersätzen

## Technologie-Stack

### Backend
- **.NET 10** (C#)
- **Entity Framework Core** (SQLite)
- **ZUGFeRD-csharp** - Bibliothek für ZUGFeRD XML Generierung
- **PdfSharpCore** - PDF Generierung
- **ASP.NET Core Web API**

### Frontend
- **React** (TypeScript)
- **React Router** für Navigation
- **Axios** für API-Aufrufe

## Projekt-Struktur

```
hsamanafahrzeug/
├── src/
│   ├── FahrzeugZulassung.Domain/        # Domain-Entitäten und Enums
│   │   ├── Entities/
│   │   │   ├── Rechnung.cs
│   │   │   ├── RechnungsPosition.cs
│   │   │   ├── Kunde.cs
│   │   │   └── Standort.cs
│   │   └── Enums/
│   │       └── RechnungFormat.cs
│   │
│   ├── FahrzeugZulassung.Infrastructure/ # Services und Datenzugriff
│   │   ├── Data/
│   │   │   └── FahrzeugZulassungDbContext.cs
│   │   └── Services/
│   │       ├── IZUGFeRDService.cs
│   │       ├── ZUGFeRDService.cs
│   │       └── ZUGFeRDResult.cs
│   │
│   └── FahrzeugZulassung.API/           # Web API
│       ├── Controllers/
│       │   ├── RechnungenController.cs
│       │   ├── KundenController.cs
│       │   └── StandorteController.cs
│       └── DTOs/
│           ├── RechnungDtos.cs
│           ├── KundeDtos.cs
│           └── StandortDtos.cs
│
├── tests/
│   └── FahrzeugZulassung.Tests/        # Unit- und Integrationstests
│       └── ZUGFeRDServiceTests.cs
│
└── frontend/                            # React Frontend
    └── src/
        ├── components/
        │   └── rechnung/
        │       ├── RechnungErstellen.tsx
        │       └── RechnungsPositionen.tsx
        ├── pages/
        │   └── RechnungsListe.tsx
        ├── services/
        │   └── api.ts
        └── types/
            └── index.ts
```

## Installation und Ausführung

### Voraussetzungen
- **.NET 10 SDK** installiert
- **Node.js** (v18+) und **npm** installiert

### Backend starten

```bash
cd src/FahrzeugZulassung.API
dotnet run
```

Die API läuft dann auf `http://localhost:5000` (HTTP) und `https://localhost:5001` (HTTPS).

### Frontend starten

```bash
cd frontend
npm install
npm start
```

Das Frontend läuft dann auf `http://localhost:3000`.

### Tests ausführen

```bash
dotnet test
```

## API-Endpoints

### Rechnungen
- `POST /api/rechnungen` - Erstellt eine neue Rechnung mit ZUGFeRD XML
- `GET /api/rechnungen` - Listet alle Rechnungen
- `GET /api/rechnungen/{id}` - Holt eine einzelne Rechnung
- `GET /api/rechnungen/{id}/pdf` - Lädt ZUGFeRD PDF herunter
- `GET /api/rechnungen/{id}/xml` - Lädt ZUGFeRD XML herunter

### Kunden
- `GET /api/kunden` - Listet alle Kunden
- `GET /api/kunden/{id}` - Holt einen einzelnen Kunden
- `POST /api/kunden` - Erstellt einen neuen Kunden

### Standorte
- `GET /api/standorte` - Listet alle Standorte
- `GET /api/standorte/{id}` - Holt einen einzelnen Standort
- `POST /api/standorte` - Erstellt einen neuen Standort

## ZUGFeRD Profile

Die Anwendung unterstützt folgende ZUGFeRD 2.1.1 Profile:

1. **Minimum** - Minimale Rechnungsinformationen
2. **Basic WL** - Basic ohne Positionen
3. **Basic** - Grundlegende Rechnungsinformationen
4. **Comfort** (EN16931) - **Empfohlen** - Vollständige elektronische Rechnung
5. **Extended** - Erweiterte Informationen

## Beispiel: Rechnung erstellen

### API Request
```http
POST /api/rechnungen
Content-Type: application/json

{
  "kundeId": "guid-des-kunden",
  "standortId": "guid-des-standorts",
  "format": 4,
  "positionen": [
    {
      "beschreibung": "Fahrzeuganmeldung",
      "menge": 1,
      "einzelpreis": 100.00,
      "steuersatz": 19
    },
    {
      "beschreibung": "Wunschkennzeichen",
      "menge": 1,
      "einzelpreis": 50.00,
      "steuersatz": 19
    }
  ]
}
```

### Response
```json
{
  "id": "guid-der-rechnung",
  "rechnungsNummer": "RE-2026-00001",
  "erstelltAm": "2026-02-16T20:00:00Z",
  "faelligkeitsdatum": "2026-03-02T20:00:00Z",
  "nettobetrag": 150.00,
  "steuerbetrag": 28.50,
  "bruttobetrag": 178.50,
  "format": 4,
  "hatZUGFeRDXml": true,
  "hatPdf": false,
  "kundeId": "guid-des-kunden",
  "kundeName": "Max Mustermann"
}
```

## Datenbank

Die Anwendung verwendet SQLite als Datenbank. Die Datenbankdatei wird automatisch bei der ersten Ausführung erstellt (`fahrzeugzulassung.db`).

## Tests

Das Projekt enthält Unit-Tests für den ZUGFeRDService:

- XML-Generierung
- XML-Validierung
- Vollständige E-Rechnungserstellung
- Korrekte Übernahme von Kunden- und Verkäuferdaten
- Korrekte Übernahme von Positionen

Alle Tests prüfen die ZUGFeRD-Konformität.

## Lizenz

MIT

## Autor

Erstellt als Teil des Fahrzeugzulassungs-Projekts mit ZUGFeRD E-Rechnung Integration.
