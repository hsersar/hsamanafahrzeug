using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.API.DTOs;

public class RechnungErstellenDto
{
    public Guid KundeId { get; set; }
    public Guid StandortId { get; set; }
    public RechnungFormat Format { get; set; } = RechnungFormat.ZUGFeRD_Comfort;
    public List<RechnungsPositionDto> Positionen { get; set; } = new();
}

public class RechnungsPositionDto
{
    public string Beschreibung { get; set; } = string.Empty;
    public string? Artikelnummer { get; set; }
    public decimal Menge { get; set; }
    public decimal Einzelpreis { get; set; }
    public decimal Steuersatz { get; set; } = 19m;
}

public class RechnungDto
{
    public Guid Id { get; set; }
    public string RechnungsNummer { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
    public DateTime Faelligkeitsdatum { get; set; }
    public decimal Nettobetrag { get; set; }
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    public RechnungFormat Format { get; set; }
    public bool HatZUGFeRDXml { get; set; }
    public bool HatPdf { get; set; }
    public Guid KundeId { get; set; }
    public string KundeName { get; set; } = string.Empty;
}
