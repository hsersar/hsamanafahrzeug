namespace FahrzeugZulassung.Application.DTOs.Provision;

public class ProvisionsBerechnung
{
    public Guid StandortId { get; set; }
    public string StandortName { get; set; } = string.Empty;
    public int Jahr { get; set; }
    public int Monat { get; set; }
    
    public int AnzahlRechnungen { get; set; }
    public int AnzahlAuftraege { get; set; }
    public decimal GesamtUmsatz { get; set; }
    
    public decimal Grundgebuehr { get; set; }
    public decimal ProvisionsProzentsatz { get; set; }
    public decimal ProvisionsBetrag { get; set; }
    
    public decimal Nettobetrag { get; set; }
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    public List<ProvisionsRechnungDetail> Rechnungsdetails { get; set; } = new();
}

public class ProvisionsRechnungDetail
{
    public Guid RechnungId { get; set; }
    public string RechnungsNummer { get; set; } = string.Empty;
    public string KundeName { get; set; } = string.Empty;
    public decimal RechnungsBetrag { get; set; }
    public decimal ProvisionsBetrag { get; set; }
    public DateTime RechnungsDatum { get; set; }
}
