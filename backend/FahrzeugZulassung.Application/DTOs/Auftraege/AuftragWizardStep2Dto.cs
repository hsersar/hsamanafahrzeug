using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auftraege;

public class AuftragWizardStep2Dto
{
    [Required(ErrorMessage = "FIN ist erforderlich")]
    [StringLength(17, MinimumLength = 17, ErrorMessage = "FIN muss genau 17 Zeichen lang sein")]
    public string FIN { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Kennzeichen darf maximal 20 Zeichen lang sein")]
    public string? Kennzeichen { get; set; }

    [Required(ErrorMessage = "Marke ist erforderlich")]
    [StringLength(100, ErrorMessage = "Marke darf maximal 100 Zeichen lang sein")]
    public string Marke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modell ist erforderlich")]
    [StringLength(100, ErrorMessage = "Modell darf maximal 100 Zeichen lang sein")]
    public string Modell { get; set; } = string.Empty;

    [Required(ErrorMessage = "Erstzulassung ist erforderlich")]
    public DateTime Erstzulassung { get; set; }
}
