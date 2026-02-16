using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Rechnungen;

public class RechnungBezahlenDto
{
    [Required(ErrorMessage = "Bezahldatum ist erforderlich")]
    public DateTime BezahltAm { get; set; }

    [StringLength(500, ErrorMessage = "Bemerkungen dürfen maximal 500 Zeichen lang sein")]
    public string? Bemerkungen { get; set; }
}
