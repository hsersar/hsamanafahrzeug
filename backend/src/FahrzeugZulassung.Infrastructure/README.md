# FahrzeugZulassung Infrastructure Layer

This document describes the complete Infrastructure layer implementation for the FahrzeugZulassung system.

## Structure

```
FahrzeugZulassung.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── SeedData.cs
│   └── Configurations/
│       ├── AuditLogConfiguration.cs
│       ├── AuftragConfiguration.cs
│       ├── BenutzerConfiguration.cs
│       ├── DokumentConfiguration.cs
│       ├── FahrzeugConfiguration.cs
│       ├── KundeConfiguration.cs
│       ├── RechnungConfiguration.cs
│       └── StandortConfiguration.cs
├── ExternalServices/
│   ├── IKfzClient.cs
│   └── IKfzModels.cs
└── FileStorage/
    └── LocalFileStorageService.cs
```

## Components

### 1. AppDbContext (`Persistence/AppDbContext.cs`)

The main database context that:
- Inherits from `IdentityDbContext<Benutzer, IdentityRole<Guid>, Guid>`
- Includes DbSets for all entities: Standorte, Kunden, Fahrzeuge, Auftraege, Rechnungen, Dokumente, AuditLogs
- Applies all entity configurations from the Configurations folder
- Overrides `SaveChangesAsync` to automatically update `GeaendertAm` timestamps on modified entities

### 2. Entity Configurations (`Persistence/Configurations/`)

Each entity has a dedicated configuration class implementing `IEntityTypeConfiguration<T>`:

#### BenutzerConfiguration
- Configures Identity user properties (Vorname, Nachname, Rolle)
- Sets up relationships with Standort, ErstellteAuftraege, and AuditLogs
- Creates indexes on Email (unique) and StandortId
- Configures delete behaviors (SetNull for Standort, Restrict for Auftraege)

#### StandortConfiguration
- Configures all Standort properties with appropriate lengths
- Sets up relationships with Mitarbeiter and Auftraege
- Creates indexes on Name, PLZ, Email, and IstAktiv
- Default value for IstAktiv = true

#### KundeConfiguration
- Configures customer personal data properties
- Sets up one-to-one relationship with Benutzer (cascade delete)
- Sets up one-to-many relationship with Auftraege (restrict delete)
- Creates indexes on BenutzerId (unique), Email, and Name fields

#### FahrzeugConfiguration
- Configures vehicle properties (FIN, Kennzeichen, Marke, etc.)
- FIN is unique and required (17 characters)
- Creates indexes on FIN (unique), Kennzeichen, and Marke/Modell
- Relationships with Auftraege (restrict delete)

#### AuftragConfiguration
- Most complex configuration with all wizard steps and status tracking
- Configures relationships with Kunde, Fahrzeug, Standort, ErstelltVon
- Cascade delete for Dokumente and Rechnungen
- Restrict delete for all other relationships
- Creates indexes on AuftragNummer (unique), Status, and all foreign keys
- Converts enums to strings (Typ, Status)

#### RechnungConfiguration
- Configures invoice properties with decimal precision (18,2) for Betrag
- Sets up relationship with Auftrag (cascade delete)
- Creates indexes on RechnungNummer (unique), Status, and dates
- Converts RechnungStatus enum to string

#### DokumentConfiguration
- Configures document metadata (Typ, DateiName, DateiPfad, ContentType, etc.)
- Relationship with Auftrag (cascade delete)
- Relationship with HochgeladenVon user (restrict delete)
- Creates indexes on AuftragId, Typ, and HochgeladenAm

#### AuditLogConfiguration
- Configures audit trail properties
- Text columns for AlteWerte and NeueWerte (JSON storage)
- Relationship with Benutzer (set null on delete)
- Multiple indexes for efficient querying: BenutzerId, Aktion, Entitaet, EntitaetId, Zeitstempel
- Composite index on (Entitaet, EntitaetId)

### 3. SeedData (`Persistence/SeedData.cs`)

Initializes the database with essential data:

#### Default Roles
- SuperAdmin
- StandortAdmin
- Mitarbeiter
- Kunde

#### Default Standort
- Name: "Hauptstandort"
- Location: München (Musterstraße 1, 80331)
- Contact: +49 89 12345678, hauptstandort@fahrzeugzulassung.de

#### SuperAdmin User
- Email: admin@fahrzeugzulassung.de
- Password: Admin@123456789
- Name: Super Admin
- Assigned to Hauptstandort
- Role: SuperAdmin

### 4. iKfz Integration (`ExternalServices/`)

#### IKfzClient Interface
Defines methods for integration with the German vehicle registration system:
- `ZulassungBeantragen` - Submit new vehicle registration
- `AbmeldungBeantragen` - Submit vehicle deregistration
- `UmschreibungBeantragen` - Submit vehicle transfer/re-registration
- `StatusAbfragen` - Query status of a submitted request

#### IKfzModels
Complete request/response models:

**Request Models:**
- `KfzZulassungRequest` - New registration
- `KfzAbmeldungRequest` - Deregistration
- `KfzUmschreibungRequest` - Transfer/re-registration

**Response Models:**
- `KfzZulassungResponse` - Registration response
- `KfzAbmeldungResponse` - Deregistration response
- `KfzUmschreibungResponse` - Transfer response
- `KfzStatusResponse` - Status query response

**Shared Models:**
- `HalterDaten` - Owner/holder data
- `FahrzeugDaten` - Vehicle data
- `DokumentReferenz` - Document reference

### 5. File Storage (`FileStorage/LocalFileStorageService.cs`)

Complete file storage implementation with:

#### Interface (`IFileStorageService`)
- `SaveFileAsync` - Save file to storage
- `GetFileAsync` - Retrieve file from storage
- `DeleteFileAsync` - Delete file from storage
- `FileExistsAsync` - Check if file exists
- `GetFileSizeAsync` - Get file size in bytes

#### Implementation Features
- Automatic directory creation organized by date (yyyy/MM/dd)
- Unique GUID-based folders to prevent conflicts
- File name sanitization (removes invalid characters)
- Automatic cleanup of empty directories after deletion
- Maximum file name length enforcement (255 characters)
- Comprehensive logging of all operations
- Async/await throughout for optimal performance

## Key Features

### 1. Type Safety
- All enums converted to strings for database storage
- Required fields properly marked
- Nullable reference types enabled
- Proper use of navigation properties

### 2. Performance
- Strategic indexes on frequently queried columns
- Composite indexes for common query patterns
- Efficient cascade delete where appropriate
- Restrict delete to prevent accidental data loss

### 3. Data Integrity
- Unique constraints on key fields (Email, FIN, AuftragNummer, etc.)
- Foreign key relationships properly configured
- Delete behaviors carefully chosen per relationship
- Automatic timestamp management

### 4. Scalability
- Date-based file organization for easy archival
- Efficient indexing strategy
- Text columns for large JSON data (AlteWerte, NeueWerte, IKfzAntwort)

### 5. Security & Compliance
- GDPR-compliant audit logging
- Comprehensive tracking of all data changes
- User action tracking with IP and UserAgent
- Automatic timestamp management

## Database Constraints Summary

### Unique Constraints
- Benutzer.Email
- Kunde.BenutzerId
- Fahrzeug.FIN
- Auftrag.AuftragNummer
- Rechnung.RechnungNummer

### Required Relationships
- Kunde → Benutzer (1:1, cascade)
- Auftrag → Kunde (restrict)
- Auftrag → Fahrzeug (restrict)
- Auftrag → Standort (restrict)
- Auftrag → ErstelltVon (restrict)
- Dokument → Auftrag (cascade)
- Rechnung → Auftrag (cascade)

### Optional Relationships
- Benutzer → Standort (set null)
- AuditLog → Benutzer (set null)

## Next Steps

To use this infrastructure layer:

1. **Add to DI Container:**
   ```csharp
   services.AddDbContext<AppDbContext>(options =>
       options.UseNpgsql(connectionString));
   
   services.AddScoped<IFileStorageService>(sp =>
       new LocalFileStorageService(
           basePath,
           sp.GetRequiredService<ILogger<LocalFileStorageService>>()
       ));
   ```

2. **Run Migrations:**
   ```bash
   dotnet ef migrations add InitialCreate --project FahrzeugZulassung.Infrastructure
   dotnet ef database update --project FahrzeugZulassung.Infrastructure
   ```

3. **Seed Initial Data:**
   ```csharp
   await SeedData.InitializeAsync(context, userManager, roleManager);
   ```

4. **Implement IKfzClient:**
   Create a concrete implementation of `IKfzClient` for your specific iKfz integration requirements.

## Build Status

✅ All files created successfully
✅ Build completed without errors or warnings
✅ Ready for migration and testing
