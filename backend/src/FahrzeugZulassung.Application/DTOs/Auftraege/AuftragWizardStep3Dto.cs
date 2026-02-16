using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auftraege;

public class AuftragWizardStep3Dto
{
    [Required(ErrorMessage = "AGB müssen akzeptiert werden")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "AGB müssen akzeptiert werden")]
    public bool AGBAkzeptiert { get; set; }

    [Required(ErrorMessage = "Datenschutzerklärung muss akzeptiert werden")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Datenschutzerklärung muss akzeptiert werden")]
    public bool DatenschutzAkzeptiert { get; set; }
}
