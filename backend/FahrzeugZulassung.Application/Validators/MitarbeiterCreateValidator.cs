using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Mitarbeiter;

namespace FahrzeugZulassung.Application.Validators;

public class MitarbeiterCreateValidator : AbstractValidator<MitarbeiterCreateDto>
{
    public MitarbeiterCreateValidator()
    {
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

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email ist erforderlich")
            .EmailAddress()
            .WithMessage("Ungültige Email-Adresse")
            .MaximumLength(255)
            .WithMessage("Email darf maximal 255 Zeichen lang sein");

        RuleFor(x => x.Telefon)
            .MaximumLength(20)
            .WithMessage("Telefonnummer darf maximal 20 Zeichen lang sein")
            .Matches(@"^[\d\s\+\-\(\)\/]+$")
            .WithMessage("Ungültige Telefonnummer")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefon));

        RuleFor(x => x.Position)
            .MaximumLength(100)
            .WithMessage("Position darf maximal 100 Zeichen lang sein")
            .When(x => !string.IsNullOrWhiteSpace(x.Position));

        RuleFor(x => x.StandortId)
            .NotEmpty()
            .WithMessage("Standort ist erforderlich");
    }
}
