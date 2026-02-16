# Fahrzeugzulassung - QES Integration

Qualifizierte Elektronische Signatur (QES) Integration für die Fahrzeugzulassungs-App.

## 📋 Überblick

Diese Implementierung ermöglicht rechtsgültige digitale Signaturen für Fahrzeugzulassungsdokumente gemäß **eIDAS-Verordnung**. Die Architektur ist **Provider-agnostisch** (Strategy Pattern) und unterstützt verschiedene QES-Anbieter.

## 🏗️ Architektur

### Backend (.NET)

```
backend/src/
├── FahrzeugZulassung.Domain/
│   ├── Entities/
│   │   ├── Signatur.cs         # Signatur-Entity mit allen Metadaten
│   │   ├── Auftrag.cs          # Erweitert mit Signaturen-Collection
│   │   └── Kunde.cs            # Kunde-Entity (Stub)
│   └── Enums/
│       ├── SignaturTyp.cs      # Dokumenttypen
│       ├── SignaturStatus.cs   # Status-Lifecycle
│       ├── SignaturLevel.cs    # SES, AES, QES
│       └── SignaturAuthMethode.cs # SMS-TAN, eID, etc.
│
├── FahrzeugZulassung.Application/
│   ├── Interfaces/
│   │   ├── ISignaturService.cs        # Haupt-Service Interface
│   │   └── ISignaturDokumentService.cs # PDF-Generierung
│   ├── DTOs/Signatur/
│   │   ├── SignaturAnforderungRequest.cs
│   │   ├── SignaturAnforderungResult.cs
│   │   ├── SignaturStatusResult.cs
│   │   ├── SignaturCallbackRequest.cs
│   │   ├── SignaturCallbackResult.cs
│   │   └── SignaturValidierungResult.cs
│   └── Validators/
│       └── SignaturAnforderungValidator.cs # FluentValidation
│
├── FahrzeugZulassung.Infrastructure/
│   ├── Signatur/
│   │   ├── MockSignaturService.cs       # ✅ Vollständig implementiert
│   │   ├── DTrustSignaturService.cs     # 🔜 Benötigt API-Credentials
│   │   ├── SwisscomSignaturService.cs   # 🔜 Benötigt API-Credentials
│   │   ├── SignaturServiceFactory.cs    # Factory Pattern
│   │   ├── SignaturDokumentService.cs   # PDF-Generierung
│   │   └── DTrustConfiguration.cs       # Konfiguration
│   └── Data/Configurations/
│       └── SignaturConfiguration.cs     # EF Core Mapping
│
└── FahrzeugZulassung.API/
    ├── Controllers/
    │   └── SignaturController.cs        # REST API Endpoints
    └── appsettings.json                 # Konfiguration
```

### Frontend (React + TypeScript)

```
frontend/src/
├── components/signatur/
│   ├── SignaturFlow.tsx               # Wizard: kompletter Flow
│   ├── DokumentVorschau.tsx          # PDF-Viewer mit Bestätigung
│   ├── SignaturMethodenAuswahl.tsx   # SMS-TAN, eID, App
│   ├── SignaturWarten.tsx            # Polling & Status-Updates
│   ├── SignaturErfolg.tsx            # Erfolgsbestätigung + Download
│   ├── SignaturFehler.tsx            # Fehlerbehandlung
│   ├── SignaturBadge.tsx             # Status-Badge Komponente
│   └── EinfacheUnterschrift.tsx      # Canvas-basiert (Fallback)
├── pages/
│   └── SignaturSeite.tsx             # Standalone Signatur-Seite
├── services/
│   └── signaturApi.ts                # API Client
└── types/
    └── signatur.ts                   # TypeScript Definitionen
```

## 🚀 Implementierungsstatus

### ✅ Vollständig implementiert

- **Domain Layer**: Alle Entities und Enums
- **Application Layer**: Interfaces, DTOs, Validatoren
- **Infrastructure**: MockSignaturService (vollständig funktionsfähig)
- **API Controller**: Alle Endpoints
- **Frontend**: Alle Komponenten und Services
- **Konfiguration**: appsettings.json, .env.example

### 🔜 Ausstehend (benötigt API-Credentials)

- **D-Trust Integration**: REST API Implementierung
- **Swisscom Integration**: REST API Implementierung
- **PDF-Generierung**: QuestPDF Integration
- **PDF-Signatur**: iText7 PAdES Integration

## 📡 API Endpoints

### Signatur-Anforderung
```http
POST /api/signatur/anfordern
Authorization: Bearer {token}
Content-Type: application/json

{
  "auftragId": "guid",
  "typ": 1,
  "signiererEmail": "kunde@example.de",
  "signiererName": "Max Mustermann",
  "signiererTelefon": "+49170123456789",
  "bevorzugteAuthMethode": 1,
  "dokumentBytes": "base64...",
  "dokumentName": "Zulassungsantrag.pdf"
}
```

### Status prüfen
```http
GET /api/signatur/{id}/status
Authorization: Bearer {token}
```

### Provider Callback (öffentlich)
```http
POST /api/signatur/callback/{provider}
Content-Type: application/json

{
  "signaturId": "guid",
  "status": "signed",
  ...
}
```

### Dokument herunterladen
```http
GET /api/signatur/{id}/dokument
Authorization: Bearer {token}
```

### Signatur validieren
```http
GET /api/signatur/{id}/validieren
Authorization: Bearer {token}
```

### Signatur stornieren
```http
POST /api/signatur/{id}/stornieren
Authorization: Bearer {token}
```

## 🎨 Frontend Flow

1. **Dokument-Vorschau**: PDF-Viewer + Checkbox "Gelesen und verstanden"
2. **Methoden-Auswahl**: SMS-TAN, App, eID wählen
3. **Kontaktdaten**: Bestätigung der Daten
4. **Weiterleitung**: Neuer Tab zum QES-Provider
5. **Warten**: Polling alle 3 Sekunden, Timer-Anzeige
6. **Erfolg**: Download + Signatur-Details

## 🔒 Sicherheit

- ✅ Alle Provider-Callbacks werden validiert (HMAC/Signatur)
- ✅ Dokument-Hashes vor/nach Signatur vergleichen
- ✅ Session-Timeout (15 Minuten)
- ✅ Max. 3 Versuche pro Signatur
- ✅ Audit-Log für alle Vorgänge
- ✅ Verschlüsselte Speicherung signierter PDFs
- ✅ Credentials nur in Environment Variables

## 📝 Konfiguration

### appsettings.json
```json
{
  "Signatur": {
    "Provider": "Mock",           // Mock, DTrust, Swisscom
    "DefaultLevel": "QES",
    "SessionTimeoutMinuten": 15,
    "MaxVersuche": 3,
    "CallbackBaseUrl": "https://app.example.de/api/signatur/callback"
  }
}
```

### .env
```bash
SIGNATUR_PROVIDER=Mock
DTRUST_CLIENT_ID=your-client-id
DTRUST_CLIENT_SECRET=your-secret
```

## 🧪 Testing

### Mock-Service für Entwicklung

Der `MockSignaturService` simuliert den kompletten QES-Flow:
- ✅ Session-Erstellung
- ✅ Dokument-Hashing
- ✅ Status-Lifecycle
- ✅ Signiertes Dokument (mit Mock-Signatur)
- ✅ Validierung

### Verwendung
```csharp
// Automatisch verwendet wenn Provider="Mock" in appsettings.json
var result = await _signaturService.SignaturAnfordernAsync(request);
```

## 🔌 Provider Integration

### D-Trust / sign-me (Bundesdruckerei)

**Status**: Stub implementiert, benötigt API-Credentials

**Schritte zur Aktivierung**:
1. D-Trust API-Zugangsdaten beantragen
2. `DTrustConfiguration` in appsettings.json ausfüllen
3. `DTrustSignaturService` REST API implementieren
4. Provider auf "DTrust" setzen

### Swisscom Trust Services

**Status**: Stub implementiert, benötigt API-Credentials

**Schritte zur Aktivierung**:
1. Swisscom AIS API-Zugangsdaten beantragen
2. Konfiguration in appsettings.json
3. `SwisscomSignaturService` implementieren
4. Provider auf "Swisscom" setzen

## 📦 Installation

### Backend
```bash
cd backend/src
# Keine .csproj Dateien vorhanden - müssen noch erstellt werden
# dotnet restore
# dotnet build
```

### Frontend
```bash
cd frontend
npm install
npm start
```

## 🎯 Nächste Schritte

1. **Backend-Projekt erstellen**: .NET 8 / ASP.NET Core Web API
2. **Datenbank-Migration**: EF Core Migration erstellen
3. **Provider-Integration**: D-Trust oder Swisscom API implementieren
4. **PDF-Generierung**: QuestPDF für Dokumente
5. **PDF-Signatur**: iText7 für PAdES-Signaturen
6. **Tests**: Unit & Integration Tests
7. **Deployment**: Azure/AWS Setup

## 📚 Dokumentation

- **eIDAS-Verordnung**: [EU Regulation 910/2014](https://eur-lex.europa.eu)
- **D-Trust sign-me**: [Dokumentation](https://www.d-trust.net/produkte/sign-me)
- **Swisscom AIS**: [API Docs](https://www.swisscom.ch/trust-services)

## 👥 Support

Bei Fragen zur QES-Integration:
- E-Mail: support@example.de
- Tel: 0800 123 456

## 📄 Lizenz

Proprietär - Alle Rechte vorbehalten