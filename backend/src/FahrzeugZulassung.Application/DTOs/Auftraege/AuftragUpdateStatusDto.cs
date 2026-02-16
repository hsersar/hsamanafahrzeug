using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auftraege;

public class AuftragUpdateStatusDto
{
    [Required(ErrorMessage = "Status ist erforderlich")]
    [StringLength(50, ErrorMessage = "Status darf maximal 50 Zeichen lang sein")]
    public string Status { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Bemerkungen dürfen maximal 1000 Zeichen lang sein")]
    public string? Bemerkungen { get; set; }
}
