using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Auftraege;

namespace FahrzeugZulassung.Application.Validators;

public class AuftragStep1Validator : AbstractValidator<AuftragWizardStep1Dto>
{
    public AuftragStep1Validator()
    {
        RuleFor(x => x.Typ)
            .NotEmpty()
            .WithMessage("Auftragstyp ist erforderlich")
            .MaximumLength(50)
            .WithMessage("Auftragstyp darf maximal 50 Zeichen lang sein")
            .Must(typ => new[] { "Anmeldung", "Abmeldung", "Ummeldung" }.Contains(typ))
            .WithMessage("Auftragstyp muss 'Anmeldung', 'Abmeldung' oder 'Ummeldung' sein");

        RuleFor(x => x.Vorname)
            .NotEmpty()
            .WithMessage("Vorname ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Vorname darf maximal 100 Zeichen lang sein")
            .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$")
            .WithMessage("Vorname darf nur Buchstaben, Leerzeichen und Bindestriche enthalten");

        RuleFor(x => x.Nachname)
            .NotEmpty()
            .WithMessage("Nachname ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Nachname darf maximal 100 Zeichen lang sein")
            .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$")
            .WithMessage("Nachname darf nur Buchstaben, Leerzeichen und Bindestriche enthalten");

        RuleFor(x => x.Strasse)
            .NotEmpty()
            .WithMessage("Strasse ist erforderlich")
            .MaximumLength(200)
            .WithMessage("Strasse darf maximal 200 Zeichen lang sein");

        RuleFor(x => x.PLZ)
            .NotEmpty()
            .WithMessage("PLZ ist erforderlich")
            .Matches(@"^\d{5}$")
            .WithMessage("PLZ muss genau 5 Ziffern enthalten");

        RuleFor(x => x.Ort)
            .NotEmpty()
            .WithMessage("Ort ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Ort darf maximal 100 Zeichen lang sein")
            .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$")
            .WithMessage("Ort darf nur Buchstaben, Leerzeichen und Bindestriche enthalten");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email ist erforderlich")
            .EmailAddress()
            .WithMessage("Ungültige Email-Adresse")
            .MaximumLength(255)
            .WithMessage("Email darf maximal 255 Zeichen lang sein");

        RuleFor(x => x.Telefon)
            .NotEmpty()
            .WithMessage("Telefon ist erforderlich")
            .MaximumLength(20)
            .WithMessage("Telefonnummer darf maximal 20 Zeichen lang sein")
            .Matches(@"^[\d\s\+\-\(\)\/]+$")
            .WithMessage("Ungültige Telefonnummer");
    }
}
