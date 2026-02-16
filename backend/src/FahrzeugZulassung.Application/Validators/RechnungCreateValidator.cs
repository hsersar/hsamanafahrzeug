using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Rechnungen;

namespace FahrzeugZulassung.Application.Validators;

public class RechnungCreateValidator : AbstractValidator<RechnungCreateDto>
{
    public RechnungCreateValidator()
    {
        RuleFor(x => x.AuftragId)
            .NotEmpty()
            .WithMessage("Auftrag ist erforderlich");

        RuleFor(x => x.Betrag)
            .NotEmpty()
            .WithMessage("Betrag ist erforderlich")
            .GreaterThan(0)
            .WithMessage("Betrag muss größer als 0 sein")
            .ScalePrecision(2, 10)
            .WithMessage("Betrag darf maximal 2 Dezimalstellen haben");

        RuleFor(x => x.Beschreibung)
            .MaximumLength(1000)
            .WithMessage("Beschreibung darf maximal 1000 Zeichen lang sein")
            .When(x => !string.IsNullOrWhiteSpace(x.Beschreibung));

        RuleFor(x => x.Faelligkeitsdatum)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Fälligkeitsdatum darf nicht in der Vergangenheit liegen")
            .When(x => x.Faelligkeitsdatum.HasValue);

        RuleFor(x => x.Positionen)
            .NotEmpty()
            .WithMessage("Mindestens eine Rechnungsposition ist erforderlich")
            .Must(positionen => positionen.Count > 0)
            .WithMessage("Mindestens eine Rechnungsposition ist erforderlich");

        RuleForEach(x => x.Positionen)
            .SetValidator(new RechnungspositionValidator());

        RuleFor(x => x)
            .Must(rechnung => ValidateTotalAmount(rechnung))
            .WithMessage("Die Summe der Positionen muss dem Rechnungsbetrag entsprechen");
    }

    private bool ValidateTotalAmount(RechnungCreateDto rechnung)
    {
        if (rechnung.Positionen == null || !rechnung.Positionen.Any())
            return false;

        var total = rechnung.Positionen.Sum(p => p.Menge * p.Einzelpreis);
        return Math.Abs(total - rechnung.Betrag) < 0.01m;
    }
}

public class RechnungspositionValidator : AbstractValidator<RechnungspositionDto>
{
    public RechnungspositionValidator()
    {
        RuleFor(x => x.Beschreibung)
            .NotEmpty()
            .WithMessage("Beschreibung ist erforderlich")
            .MaximumLength(500)
            .WithMessage("Beschreibung darf maximal 500 Zeichen lang sein");

        RuleFor(x => x.Menge)
            .GreaterThan(0)
            .WithMessage("Menge muss größer als 0 sein");

        RuleFor(x => x.Einzelpreis)
            .GreaterThan(0)
            .WithMessage("Einzelpreis muss größer als 0 sein")
            .ScalePrecision(2, 10)
            .WithMessage("Einzelpreis darf maximal 2 Dezimalstellen haben");
    }
}
