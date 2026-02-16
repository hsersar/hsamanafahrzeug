namespace FahrzeugZulassung.Domain.Entities;

/// <summary>
/// Optionale Staffelung: ab einem bestimmten Umsatz sinkt/steigt der Provisionssatz
/// </summary>
public class ProvisionsStaffel
{
    public Guid Id { get; set; }
    public Guid ProvisionsModellId { get; set; }
    
    public decimal AbUmsatz { get; set; }           // Ab diesem Monatsumsatz
    public decimal BisUmsatz { get; set; }          // Bis zu diesem Monatsumsatz
    public decimal Prozentsatz { get; set; }        // Provisionssatz für diese Staffel
    
    // Navigation
    public ProvisionsModell ProvisionsModell { get; set; } = null!;
}
