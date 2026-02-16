using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Auftraege;

namespace FahrzeugZulassung.Application.Validators;

public class AuftragStep3Validator : AbstractValidator<AuftragWizardStep3Dto>
{
    public AuftragStep3Validator()
    {
        RuleFor(x => x.AGBAkzeptiert)
            .Equal(true)
            .WithMessage("Die Allgemeinen Geschäftsbedingungen müssen akzeptiert werden");

        RuleFor(x => x.DatenschutzAkzeptiert)
            .Equal(true)
            .WithMessage("Die Datenschutzerklärung muss akzeptiert werden");
    }
}
