namespace FahrzeugZulassung.Domain.Entities;

public class Auftrag
{
    public Guid Id { get; set; }
    // ... other properties
    
    // Navigation
    public ICollection<Signatur> Signaturen { get; set; } = new List<Signatur>();
    public bool AlleSignaturenVorhanden => Signaturen.All(s => s.Status == Enums.SignaturStatus.Signiert);
}
