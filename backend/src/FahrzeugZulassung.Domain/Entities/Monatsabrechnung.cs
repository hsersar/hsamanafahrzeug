using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Monatsabrechnung
{
    public Guid Id { get; set; }
    public Guid StandortId { get; set; }
    
    // Abrechnungszeitraum
    public int Jahr { get; set; }
    public int Monat { get; set; }
    public DateTime PeriodeVon { get; set; }       // 1. des Monats
    public DateTime PeriodeBis { get; set; }       // Letzter Tag des Monats
    
    // Rechnungsdaten
    public string AbrechnungsNummer { get; set; } = string.Empty;  // z.B. "ABR-2026-02-001"
    
    // Berechnungsgrundlage
    public int AnzahlRechnungen { get; set; }      // Anzahl der Kunden-Rechnungen im Monat
    public int AnzahlAuftraege { get; set; }       // Anzahl der Aufträge im Monat
    public decimal GesamtUmsatz { get; set; }      // Gesamtumsatz des Standorts im Monat
    
    // Gebühren
    public decimal Grundgebuehr { get; set; }      // Feste monatliche Grundgebühr
    public decimal ProvisionsProzentsatz { get; set; }  // Angewandter Prozentsatz
    public decimal ProvisionsBetrag { get; set; }       // Berechnete Provision
    
    // Summen
    public decimal Nettobetrag { get; set; }       // Grundgebühr + Provision
    public decimal Steuersatz { get; set; } = 19m;
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    // Status
    public MonatsabrechnungStatus Status { get; set; } = MonatsabrechnungStatus.Entwurf;
    
    // Zahlung
    public DateTime? Faelligkeitsdatum { get; set; }     // 14 Tage nach Erstellung
    public DateTime? BezahltAm { get; set; }
    public string? ZahlungsReferenz { get; set; }
    
    // ZUGFeRD (die Abrechnung wird ebenfalls als E-Rechnung erstellt!)
    public string? ZUGFeRDXml { get; set; }
    public string? PdfDateiPfad { get; set; }
    
    // Audit
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public string? ErstelltVon { get; set; }
    public DateTime? VersandtAm { get; set; }         // Wann wurde sie an den Standort geschickt
    
    // Navigation
    public Standort Standort { get; set; } = null!;
    public ICollection<MonatsabrechnungPosition> Positionen { get; set; } = new List<MonatsabrechnungPosition>();
}
