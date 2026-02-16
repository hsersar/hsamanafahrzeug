using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Auftraege;
using System.Text.RegularExpressions;

namespace FahrzeugZulassung.Application.Validators;

public class AuftragStep2Validator : AbstractValidator<AuftragWizardStep2Dto>
{
    public AuftragStep2Validator()
    {
        RuleFor(x => x.FIN)
            .NotEmpty()
            .WithMessage("FIN (Fahrzeug-Identifizierungsnummer) ist erforderlich")
            .Length(17)
            .WithMessage("FIN muss genau 17 Zeichen lang sein")
            .Must(BeValidFIN)
            .WithMessage("FIN enthält ungültige Zeichen. Erlaubt sind: A-Z, 0-9 (ohne I, O, Q)");

        RuleFor(x => x.Kennzeichen)
            .MaximumLength(20)
            .WithMessage("Kennzeichen darf maximal 20 Zeichen lang sein")
            .Matches(@"^[A-ZÄÖÜ]{1,3}\s?[A-Z]{1,2}\s?\d{1,4}[EH]?$")
            .WithMessage("Ungültiges deutsches Kennzeichen-Format")
            .When(x => !string.IsNullOrWhiteSpace(x.Kennzeichen));

        RuleFor(x => x.Marke)
            .NotEmpty()
            .WithMessage("Marke ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Marke darf maximal 100 Zeichen lang sein");

        RuleFor(x => x.Modell)
            .NotEmpty()
            .WithMessage("Modell ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Modell darf maximal 100 Zeichen lang sein");

        RuleFor(x => x.Erstzulassung)
            .NotEmpty()
            .WithMessage("Erstzulassung ist erforderlich")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Erstzulassung darf nicht in der Zukunft liegen")
            .GreaterThan(new DateTime(1900, 1, 1))
            .WithMessage("Erstzulassung muss nach dem 01.01.1900 liegen");
    }

    private bool BeValidFIN(string fin)
    {
        if (string.IsNullOrWhiteSpace(fin) || fin.Length != 17)
            return false;

        // FIN darf nicht die Buchstaben I, O, Q enthalten (Verwechslungsgefahr mit 1, 0)
        return Regex.IsMatch(fin, @"^[A-HJ-NPR-Z0-9]{17}$");
    }
}
