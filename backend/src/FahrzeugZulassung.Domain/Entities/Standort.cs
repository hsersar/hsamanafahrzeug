namespace FahrzeugZulassung.Domain.Entities;

public class Standort
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public bool IstAktiv { get; set; } = true;
    
    // Navigation properties
    public ICollection<ProvisionsModell>? ProvisionsModelle { get; set; }
    public ICollection<Monatsabrechnung>? Monatsabrechnungen { get; set; }
}
