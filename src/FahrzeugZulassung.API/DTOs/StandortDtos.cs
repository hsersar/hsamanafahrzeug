namespace FahrzeugZulassung.API.DTOs;

public class StandortDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Firmenname { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? UStID { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
}

public class StandortErstellenDto
{
    public string Name { get; set; } = string.Empty;
    public string Firmenname { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? UStID { get; set; }
    public string? Steuernummer { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? Bankname { get; set; }
}
