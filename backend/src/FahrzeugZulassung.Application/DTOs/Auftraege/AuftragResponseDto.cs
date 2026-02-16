namespace FahrzeugZulassung.Application.DTOs.Auftraege;

public class AuftragResponseDto
{
    public Guid Id { get; set; }
    public string Typ { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
    public DateTime? AbgeschlossenAm { get; set; }
    public string? Bemerkungen { get; set; }
    public decimal? Preis { get; set; }
    public bool AGBAkzeptiert { get; set; }
    public bool DatenschutzAkzeptiert { get; set; }

    public KundeInfoDto Kunde { get; set; } = new();
    public FahrzeugInfoDto Fahrzeug { get; set; } = new();
    public StandortInfoDto? Standort { get; set; }
    public MitarbeiterInfoDto? Mitarbeiter { get; set; }
}

public class KundeInfoDto
{
    public Guid Id { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
}

public class FahrzeugInfoDto
{
    public Guid Id { get; set; }
    public string FIN { get; set; } = string.Empty;
    public string? Kennzeichen { get; set; }
    public string Marke { get; set; } = string.Empty;
    public string Modell { get; set; } = string.Empty;
    public DateTime Erstzulassung { get; set; }
}

public class StandortInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
}

public class MitarbeiterInfoDto
{
    public Guid Id { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
