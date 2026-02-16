# QES Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           FRONTEND (React + TypeScript)                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                         SignaturSeite.tsx                             │  │
│  │                    (Standalone Signature Page)                        │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
│  ┌────────────────────────────────▼─────────────────────────────────────┐  │
│  │                        SignaturFlow.tsx                               │  │
│  │                      (Main Wizard Component)                          │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ Step 1: DokumentVorschau    → PDF Viewer + Checkbox                  │  │
│  │ Step 2: MethodenAuswahl     → SMS-TAN | eID | App | Video            │  │
│  │ Step 3: Kontaktdaten        → Name, Email, Phone confirmation        │  │
│  │ Step 4: Weiterleitung       → Open provider in new tab               │  │
│  │ Step 5: SignaturWarten      → Polling (3s) + Timer (15min)           │  │
│  │ Step 6: SignaturErfolg      → Download + Certificate info            │  │
│  │         SignaturFehler      → Error handling + retry                 │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
│  ┌────────────────────────────────▼─────────────────────────────────────┐  │
│  │                      SignaturApi (API Client)                         │  │
│  │  • signaturAnfordern()      • downloadSigniertesDokument()           │  │
│  │  • getStatus()              • validieren()                            │  │
│  │  • pollStatus()             • stornieren()                            │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
└───────────────────────────────────┼─────────────────────────────────────────┘
                                    │
                                    │ HTTPS / REST API
                                    │
┌───────────────────────────────────▼─────────────────────────────────────────┐
│                          BACKEND (.NET / C#)                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                   SignaturController (API Layer)                      │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ POST   /api/signatur/anfordern          [Authorize: Kunde]           │  │
│  │ GET    /api/signatur/{id}/status        [Authorize]                  │  │
│  │ POST   /api/signatur/callback/{provider} [AllowAnonymous]            │  │
│  │ GET    /api/signatur/{id}/dokument      [Authorize]                  │  │
│  │ GET    /api/signatur/{id}/validieren    [Authorize]                  │  │
│  │ POST   /api/signatur/{id}/stornieren    [Authorize: Mitarbeiter]     │  │
│  │ GET    /api/signatur/{id}/redirect      [AllowAnonymous]             │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
│  ┌────────────────────────────────▼─────────────────────────────────────┐  │
│  │              Application Layer (DTOs & Validators)                    │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ • SignaturAnforderungRequest/Result                                  │  │
│  │ • SignaturStatusResult                                               │  │
│  │ • SignaturCallbackRequest/Result                                     │  │
│  │ • SignaturValidierungResult                                          │  │
│  │ • SignaturAnforderungValidator (FluentValidation)                    │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
│  ┌────────────────────────────────▼─────────────────────────────────────┐  │
│  │                   ISignaturService Interface                          │  │
│  │  • SignaturAnfordernAsync()     • CallbackVerarbeitenAsync()         │  │
│  │  • StatusPruefenAsync()         • SigniertesDokumentAbrufenAsync()   │  │
│  │  • SignaturValidierenAsync()    • StornierenAsync()                  │  │
│  └────────────────────────────────┬─────────────────────────────────────┘  │
│                                   │                                         │
│  ┌────────────────────────────────▼─────────────────────────────────────┐  │
│  │              SignaturServiceFactory (Strategy Pattern)                │  │
│  └────┬────────────────────┬────────────────────┬────────────────────┬──┘  │
│       │                    │                    │                    │     │
│  ┌────▼────────┐     ┌────▼────────┐     ┌────▼────────┐     ┌────▼────┐ │
│  │    Mock     │     │   D-Trust   │     │  Swisscom   │     │  Future │ │
│  │  Signatur   │     │  sign-me    │     │    Trust    │     │ Provider│ │
│  │  Service    │     │  Service    │     │   Service   │     │         │ │
│  ├─────────────┤     ├─────────────┤     ├─────────────┤     └─────────┘ │
│  │✅ COMPLETE  │     │🔜 Stub      │     │🔜 Stub      │                  │
│  │Full QES    │     │Needs API    │     │Needs API    │                  │
│  │simulation  │     │credentials  │     │credentials  │                  │
│  └─────────────┘     └─────────────┘     └─────────────┘                  │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                        Domain Layer                                   │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ Entities:                                                             │  │
│  │  • Signatur (40+ properties, full audit trail)                       │  │
│  │  • Auftrag (with Signaturen collection)                              │  │
│  │  • Kunde                                                              │  │
│  │                                                                        │  │
│  │ Enums:                                                                │  │
│  │  • SignaturTyp      (7 types: Zulassungsantrag, Vollmacht, etc.)    │  │
│  │  • SignaturStatus   (10 states: Angefordert → Signiert)             │  │
│  │  • SignaturLevel    (SES, AES, QES)                                  │  │
│  │  • SignaturAuthMethode (SMS-TAN, eID, App, Video)                   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │               Infrastructure - Data (EF Core)                         │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ • SignaturConfiguration (Entity mapping)                             │  │
│  │ • Indexes: ProviderSessionId, ProviderTransaktionId, AuftragId       │  │
│  │ • Foreign Keys: Auftrag (Cascade), Kunde (SetNull)                   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
└──────────────────────────────────┬───────────────────────────────────────────┘
                                   │
                                   │ Database Connection
                                   │
┌──────────────────────────────────▼───────────────────────────────────────────┐
│                           DATABASE (PostgreSQL/SQL)                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                          Signaturen Table                             │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ PK: Id (UUID)                                                         │  │
│  │ FK: AuftragId → Auftraege.Id (CASCADE)                               │  │
│  │ FK: KundeId → Kunden.Id (SET NULL)                                   │  │
│  │                                                                        │  │
│  │ Metadata: Typ, Status, Level, Provider                               │  │
│  │ Document: Name, Hash, Paths                                           │  │
│  │ Provider: SessionId, TransaktionId, URLs                              │  │
│  │ Certificate: Subject, Issuer, Serial, Validity, Value                │  │
│  │ Auth: Methode, SigniererName, Email                                  │  │
│  │ Timestamps: Erstellt, Angefordert, Signiert, Abgelaufen              │  │
│  │ Audit: FehlerNachricht, ProviderAntwort, Versuche                    │  │
│  │                                                                        │  │
│  │ Indexes:                                                              │  │
│  │  • idx_provider_session (ProviderSessionId)                          │  │
│  │  • idx_provider_transaktion (ProviderTransaktionId)                  │  │
│  │  • idx_auftrag (AuftragId)                                           │  │
│  │  • idx_status (Status)                                               │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                    EXTERNAL QES PROVIDERS (TSP)                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐        │
│  │    D-Trust       │   │    Swisscom      │   │     DocuSign     │        │
│  │   sign-me        │   │  Trust Services  │   │                  │        │
│  │ (Bundesdruckerei)│   │                  │   │                  │        │
│  ├──────────────────┤   ├──────────────────┤   ├──────────────────┤        │
│  │ • SMS-TAN        │   │ • Mobile ID      │   │ • QES Cloud      │        │
│  │ • eID            │   │ • SMS-TAN        │   │ • Standards      │        │
│  │ • Video-Ident    │   │ • Batch signing  │   │                  │        │
│  │ • PAdES/CAdES    │   │ • PAdES/CAdES    │   │                  │        │
│  │ • eIDAS-cert.    │   │ • eIDAS-rec.     │   │ • eIDAS-conf.    │        │
│  └──────────────────┘   └──────────────────┘   └──────────────────┘        │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                         SIGNATURE FLOW SEQUENCE                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Frontend          API              Service           Provider              │
│     │               │                  │                  │                 │
│     │──Anfordern──>│                  │                  │                 │
│     │               │──Create Session─>│                  │                 │
│     │               │                  │──Request Sign──>│                 │
│     │               │                  │<──Redirect URL───│                 │
│     │               │<──Result+URL─────│                  │                 │
│     │<──Result──────│                  │                  │                 │
│     │               │                  │                  │                 │
│     │──Open URL────────────────────────────────────────>│                 │
│     │               │                  │                  │                 │
│     │               │                  │                  │──[User Auth]──  │
│     │               │                  │                  │                 │
│     │               │<─────Callback───────────────────────│                 │
│     │               │──Process─────────>│                  │                 │
│     │               │                  │──Get Document──>│                 │
│     │               │                  │<──Signed PDF─────│                 │
│     │               │<──Success─────────│                  │                 │
│     │               │                  │                  │                 │
│     │──Poll Status─>│                  │                  │                 │
│     │<──Signiert────│                  │                  │                 │
│     │               │                  │                  │                 │
│     │──Download────>│                  │                  │                 │
│     │<──PDF─────────│                  │                  │                 │
│     │               │                  │                  │                 │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Legend

- ✅ **Complete**: Fully implemented and functional
- 🔜 **Stub**: Interface defined, needs implementation
- → **Data Flow**: Direction of information
- │ **Dependency**: Component relationship
- ├─ **Has-A**: Composition relationship
- PK/FK: Primary/Foreign Key
- TSP: Trust Service Provider (Vertrauensdiensteanbieter)
