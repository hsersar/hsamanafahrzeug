namespace FahrzeugZulassung.Domain.Entities;

public class ProvisionsModell
{
    public Guid Id { get; set; }
    
    // Standard-Werte (von SuperAdmin definiert, gelten für alle neuen Standorte)
    public bool IstStandard { get; set; }   // true = globaler Standard
    
    // Zuordnung
    public Guid? StandortId { get; set; }    // null = Standard-Modell
    
    // Gebühren
    public decimal MonatlicheGrundgebuehr { get; set; } = 199.00m;
    public decimal ProvisionsProzentsatz { get; set; } = 2.0m;       // 2% Standard
    public decimal? MinProvisionProRechnung { get; set; }             // Optional: Mindestprovision pro Rechnung
    public decimal? MaxProvisionProRechnung { get; set; }             // Optional: Maximalprovision pro Rechnung
    
    // Gültigkeit
    public DateTime GueltigAb { get; set; } = DateTime.UtcNow;
    public DateTime? GueltigBis { get; set; }
    public bool IstAktiv { get; set; } = true;
    
    // Staffelung (optional)
    public bool HatStaffelung { get; set; } = false;
    
    // Audit
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? AktualisiertAm { get; set; }
    public string? ErstelltVon { get; set; }           // BenutzerName
    public string? AktualisiertVon { get; set; }
    public string? Aenderungsgrund { get; set; }       // Warum wurde der Satz geändert
    
    // Navigation
    public Standort? Standort { get; set; }
    public ICollection<ProvisionsStaffel>? Staffeln { get; set; }
}
