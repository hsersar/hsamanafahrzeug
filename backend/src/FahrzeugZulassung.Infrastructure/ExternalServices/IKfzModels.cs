namespace FahrzeugZulassung.Infrastructure.ExternalServices;

// Request Models
public class KfzZulassungRequest
{
    public required string FIN { get; set; }
    public string? Kennzeichen { get; set; }
    public required HalterDaten Halter { get; set; }
    public required FahrzeugDaten Fahrzeug { get; set; }
    public List<DokumentReferenz> Dokumente { get; set; } = new();
    public required string StandortKennung { get; set; }
}

public class KfzAbmeldungRequest
{
    public required string Kennzeichen { get; set; }
    public required string FIN { get; set; }
    public required HalterDaten Halter { get; set; }
    public string? Grund { get; set; }
    public DateTime AbmeldungDatum { get; set; }
    public required string StandortKennung { get; set; }
}

public class KfzUmschreibungRequest
{
    public required string Kennzeichen { get; set; }
    public required string FIN { get; set; }
    public required HalterDaten AlterHalter { get; set; }
    public required HalterDaten NeuerHalter { get; set; }
    public required FahrzeugDaten Fahrzeug { get; set; }
    public List<DokumentReferenz> Dokumente { get; set; } = new();
    public required string StandortKennung { get; set; }
}

// Response Models
public class KfzZulassungResponse
{
    public bool Erfolg { get; set; }
    public string? Referenz { get; set; }
    public string? Kennzeichen { get; set; }
    public string? Status { get; set; }
    public string? Nachricht { get; set; }
    public List<string> Fehler { get; set; } = new();
    public DateTime? BearbeitungsDatum { get; set; }
}

public class KfzAbmeldungResponse
{
    public bool Erfolg { get; set; }
    public string? Referenz { get; set; }
    public string? Status { get; set; }
    public string? Nachricht { get; set; }
    public List<string> Fehler { get; set; } = new();
    public DateTime? AbmeldungDatum { get; set; }
}

public class KfzUmschreibungResponse
{
    public bool Erfolg { get; set; }
    public string? Referenz { get; set; }
    public string? NeuesKennzeichen { get; set; }
    public string? Status { get; set; }
    public string? Nachricht { get; set; }
    public List<string> Fehler { get; set; } = new();
    public DateTime? BearbeitungsDatum { get; set; }
}

public class KfzStatusResponse
{
    public bool Erfolg { get; set; }
    public string? Referenz { get; set; }
    public string? Status { get; set; }
    public string? Nachricht { get; set; }
    public List<string> Fehler { get; set; } = new();
    public DateTime? LetzteAktualisierung { get; set; }
    public bool IstAbgeschlossen { get; set; }
}

// Shared Data Models
public class HalterDaten
{
    public required string Vorname { get; set; }
    public required string Nachname { get; set; }
    public required string Strasse { get; set; }
    public required string Hausnummer { get; set; }
    public required string PLZ { get; set; }
    public required string Ort { get; set; }
    public DateTime? Geburtsdatum { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
}

public class FahrzeugDaten
{
    public required string FIN { get; set; }
    public string? Kennzeichen { get; set; }
    public required string Marke { get; set; }
    public required string Modell { get; set; }
    public DateTime? Erstzulassung { get; set; }
    public string? Farbe { get; set; }
    public int? Hubraum { get; set; }
    public int? Leistung { get; set; }
    public string? Kraftstoffart { get; set; }
}

public class DokumentReferenz
{
    public required string Typ { get; set; }
    public required string DateiName { get; set; }
    public required string DateiPfad { get; set; }
    public string? ContentType { get; set; }
}
