using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Provision;

namespace FahrzeugZulassung.Application.Validators;

public class ProvisionsModellUpdateValidator : AbstractValidator<ProvisionsModellUpdateDto>
{
    public ProvisionsModellUpdateValidator()
    {
        RuleFor(x => x.MonatlicheGrundgebuehr)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Grundgebühr darf nicht negativ sein.");
        
        RuleFor(x => x.ProvisionsProzentsatz)
            .InclusiveBetween(0, 100)
            .WithMessage("Provisionssatz muss zwischen 0% und 100% liegen.");
        
        RuleFor(x => x.MinProvisionProRechnung)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinProvisionProRechnung.HasValue);
        
        RuleFor(x => x.MaxProvisionProRechnung)
            .GreaterThan(x => x.MinProvisionProRechnung ?? 0)
            .When(x => x.MaxProvisionProRechnung.HasValue)
            .WithMessage("Max-Provision muss größer als Min-Provision sein.");
        
        RuleFor(x => x.Aenderungsgrund)
            .NotEmpty()
            .WithMessage("Bitte geben Sie einen Grund für die Änderung an.");
    }
}
