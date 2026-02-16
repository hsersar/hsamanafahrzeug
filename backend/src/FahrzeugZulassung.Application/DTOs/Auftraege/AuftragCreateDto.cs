using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auftraege;

public class AuftragCreateDto
{
    [Required(ErrorMessage = "Auftragstyp ist erforderlich")]
    [StringLength(50, ErrorMessage = "Auftragstyp darf maximal 50 Zeichen lang sein")]
    public string Typ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Vorname darf maximal 100 Zeichen lang sein")]
    public string KundeVorname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Nachname darf maximal 100 Zeichen lang sein")]
    public string KundeNachname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Strasse ist erforderlich")]
    [StringLength(200, ErrorMessage = "Strasse darf maximal 200 Zeichen lang sein")]
    public string KundeStrasse { get; set; } = string.Empty;

    [Required(ErrorMessage = "PLZ ist erforderlich")]
    [StringLength(10, ErrorMessage = "PLZ darf maximal 10 Zeichen lang sein")]
    public string KundePLZ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ort ist erforderlich")]
    [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
    public string KundeOrt { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
    [StringLength(255, ErrorMessage = "Email darf maximal 255 Zeichen lang sein")]
    public string KundeEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon ist erforderlich")]
    [Phone(ErrorMessage = "Ungültige Telefonnummer")]
    [StringLength(20, ErrorMessage = "Telefonnummer darf maximal 20 Zeichen lang sein")]
    public string KundeTelefon { get; set; } = string.Empty;

    [Required(ErrorMessage = "FIN ist erforderlich")]
    [StringLength(17, MinimumLength = 17, ErrorMessage = "FIN muss genau 17 Zeichen lang sein")]
    public string FahrzeugFIN { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Kennzeichen darf maximal 20 Zeichen lang sein")]
    public string? FahrzeugKennzeichen { get; set; }

    [Required(ErrorMessage = "Marke ist erforderlich")]
    [StringLength(100, ErrorMessage = "Marke darf maximal 100 Zeichen lang sein")]
    public string FahrzeugMarke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modell ist erforderlich")]
    [StringLength(100, ErrorMessage = "Modell darf maximal 100 Zeichen lang sein")]
    public string FahrzeugModell { get; set; } = string.Empty;

    [Required(ErrorMessage = "Erstzulassung ist erforderlich")]
    public DateTime FahrzeugErstzulassung { get; set; }

    [Required(ErrorMessage = "AGB müssen akzeptiert werden")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "AGB müssen akzeptiert werden")]
    public bool AGBAkzeptiert { get; set; }

    [Required(ErrorMessage = "Datenschutzerklärung muss akzeptiert werden")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Datenschutzerklärung muss akzeptiert werden")]
    public bool DatenschutzAkzeptiert { get; set; }

    public Guid? StandortId { get; set; }
}
