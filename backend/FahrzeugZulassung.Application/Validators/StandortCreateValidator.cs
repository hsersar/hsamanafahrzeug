using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Standorte;

namespace FahrzeugZulassung.Application.Validators;

public class StandortCreateValidator : AbstractValidator<StandortCreateDto>
{
    public StandortCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name ist erforderlich")
            .MaximumLength(200)
            .WithMessage("Name darf maximal 200 Zeichen lang sein");

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

        RuleFor(x => x.Telefon)
            .MaximumLength(20)
            .WithMessage("Telefonnummer darf maximal 20 Zeichen lang sein")
            .Matches(@"^[\d\s\+\-\(\)\/]+$")
            .WithMessage("Ungültige Telefonnummer")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefon));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Ungültige Email-Adresse")
            .MaximumLength(255)
            .WithMessage("Email darf maximal 255 Zeichen lang sein")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
