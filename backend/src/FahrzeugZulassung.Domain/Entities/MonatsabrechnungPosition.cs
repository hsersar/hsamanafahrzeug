using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class MonatsabrechnungPosition
{
    public Guid Id { get; set; }
    public Guid MonatsabrechnungId { get; set; }
    
    public int Position { get; set; }
    public MonatsabrechnungPositionTyp Typ { get; set; }
    public string Beschreibung { get; set; } = string.Empty;
    
    public decimal Menge { get; set; }             // z.B. 1 (Grundgebühr) oder 50 (Anzahl Rechnungen)
    public string Einheit { get; set; } = "Stk";
    public decimal Einzelpreis { get; set; }
    public decimal Nettobetrag { get; set; }
    public decimal Steuersatz { get; set; } = 19m;
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    // Referenz zu Kunden-Rechnungen (für Provision)
    public Guid? ReferenzRechnungId { get; set; }   // Optional: Verweis auf die Kunden-Rechnung
    
    // Navigation
    public Monatsabrechnung Monatsabrechnung { get; set; } = null!;
}
