using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Dokument
{
    public Guid Id { get; set; }
    
    public Guid AuftragId { get; set; }
    public Auftrag Auftrag { get; set; } = null!;
    
    public DokumentTyp Typ { get; set; }
    public string DateiName { get; set; } = string.Empty;
    public string DateiPfad { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long DateiGroesse { get; set; }
    
    public Guid HochgeladenVonId { get; set; }
    public Benutzer HochgeladenVon { get; set; } = null!;
    
    public DateTime HochgeladenAm { get; set; } = DateTime.UtcNow;
}
