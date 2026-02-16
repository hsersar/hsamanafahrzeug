using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs.Provision;

public class PlattformUmsatzDto
{
    public int Jahr { get; set; }
    public int? Monat { get; set; }
    
    public int AnzahlStandorte { get; set; }
    public int AnzahlAktiveStandorte { get; set; }
    
    public decimal GesamtUmsatzAllerStandorte { get; set; }
    public decimal GesamtGrundgebuehren { get; set; }
    public decimal GesamtProvisionen { get; set; }
    public decimal GesamtPlattformEinnahmen { get; set; }
    
    public decimal OffeneAbrechnungen { get; set; }
    public decimal BezahlteAbrechnungen { get; set; }
    public decimal UeberfaelligeAbrechnungen { get; set; }
    
    public List<StandortUmsatzDetail> StandortDetails { get; set; } = new();
}

public class StandortUmsatzDetail
{
    public Guid StandortId { get; set; }
    public string StandortName { get; set; } = string.Empty;
    public decimal Umsatz { get; set; }
    public decimal Provision { get; set; }
    public decimal Grundgebuehr { get; set; }
    public decimal GesamtAbrechnung { get; set; }
    public MonatsabrechnungStatus AbrechnungStatus { get; set; }
    public int AnzahlAuftraege { get; set; }
}
