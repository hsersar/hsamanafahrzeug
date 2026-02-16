namespace FahrzeugZulassung.Domain.Entities;

public class RechnungsPosition
{
    public Guid Id { get; set; }
    public Guid RechnungId { get; set; }
    public int Position { get; set; }
    public string Beschreibung { get; set; } = string.Empty;
    public string? Artikelnummer { get; set; }
    public decimal Menge { get; set; }
    public string Einheit { get; set; } = "C62";  // UN/ECE Recommendation 20 code
    public decimal Einzelpreis { get; set; }
    public decimal Nettobetrag { get; set; }
    public decimal Steuersatz { get; set; } = 19m; // MwSt in %
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    // Navigation
    public Rechnung Rechnung { get; set; } = null!;
}
