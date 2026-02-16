using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs.Provision;

public class MonatsabrechnungDto
{
    public Guid Id { get; set; }
    public Guid StandortId { get; set; }
    public string StandortName { get; set; } = string.Empty;
    public string AbrechnungsNummer { get; set; } = string.Empty;
    
    public int Jahr { get; set; }
    public int Monat { get; set; }
    public string Zeitraum { get; set; } = string.Empty;  // "Februar 2026"
    
    public int AnzahlRechnungen { get; set; }
    public int AnzahlAuftraege { get; set; }
    public decimal GesamtUmsatz { get; set; }
    
    public decimal Grundgebuehr { get; set; }
    public decimal ProvisionsProzentsatz { get; set; }
    public decimal ProvisionsBetrag { get; set; }
    
    public decimal Nettobetrag { get; set; }
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    public MonatsabrechnungStatus Status { get; set; }
    public DateTime? Faelligkeitsdatum { get; set; }
    public DateTime? BezahltAm { get; set; }
    
    public List<MonatsabrechnungPositionDto> Positionen { get; set; } = new();
}

public class MonatsabrechnungPositionDto
{
    public Guid Id { get; set; }
    public int Position { get; set; }
    public MonatsabrechnungPositionTyp Typ { get; set; }
    public string Beschreibung { get; set; } = string.Empty;
    public decimal Menge { get; set; }
    public string Einheit { get; set; } = string.Empty;
    public decimal Einzelpreis { get; set; }
    public decimal Nettobetrag { get; set; }
    public decimal Steuersatz { get; set; }
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
}
