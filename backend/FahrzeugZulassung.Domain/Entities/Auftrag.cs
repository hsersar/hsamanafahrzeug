using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Auftrag
{
    public Guid Id { get; set; }
    public string AuftragNummer { get; set; } = string.Empty;
    
    public AuftragTyp Typ { get; set; }
    public AuftragStatus Status { get; set; } = AuftragStatus.Entwurf;
    
    // Relations
    public Guid KundeId { get; set; }
    public Kunde Kunde { get; set; } = null!;
    
    public Guid FahrzeugId { get; set; }
    public Fahrzeug Fahrzeug { get; set; } = null!;
    
    public Guid StandortId { get; set; }
    public Standort Standort { get; set; } = null!;
    
    public Guid ErstelltVonId { get; set; }
    public Benutzer ErstelltVon { get; set; } = null!;
    
    // Wizard data
    public bool Step1Abgeschlossen { get; set; } = false;
    public bool Step2Abgeschlossen { get; set; } = false;
    public bool Step3Abgeschlossen { get; set; } = false;
    
    public bool AGBAkzeptiert { get; set; } = false;
    public DateTime? AGBAkzeptiertAm { get; set; }
    
    public bool UnterschriftVorhanden { get; set; } = false;
    public DateTime? UnterschriftAm { get; set; }
    
    // iKFZ Integration
    public string? IKfzReferenz { get; set; }
    public DateTime? AnIKfzGesendetAm { get; set; }
    public string? IKfzAntwort { get; set; }
    
    public string? Bemerkungen { get; set; }
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
    public DateTime? AbgeschlossenAm { get; set; }
    
    // Navigation properties
    public ICollection<Dokument> Dokumente { get; set; } = new List<Dokument>();
    public ICollection<Rechnung> Rechnungen { get; set; } = new List<Rechnung>();
}
