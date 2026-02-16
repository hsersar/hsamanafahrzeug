using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Rechnungen;

public class RechnungCreateDto
{
    [Required(ErrorMessage = "Auftrag ist erforderlich")]
    public Guid AuftragId { get; set; }

    [Required(ErrorMessage = "Betrag ist erforderlich")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Betrag muss größer als 0 sein")]
    public decimal Betrag { get; set; }

    [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
    public string? Beschreibung { get; set; }

    public DateTime? Faelligkeitsdatum { get; set; }

    public List<RechnungspositionDto> Positionen { get; set; } = new();
}

public class RechnungspositionDto
{
    [Required(ErrorMessage = "Beschreibung ist erforderlich")]
    [StringLength(500, ErrorMessage = "Beschreibung darf maximal 500 Zeichen lang sein")]
    public string Beschreibung { get; set; } = string.Empty;

    [Required(ErrorMessage = "Menge ist erforderlich")]
    [Range(1, int.MaxValue, ErrorMessage = "Menge muss größer als 0 sein")]
    public int Menge { get; set; }

    [Required(ErrorMessage = "Einzelpreis ist erforderlich")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Einzelpreis muss größer als 0 sein")]
    public decimal Einzelpreis { get; set; }
}
