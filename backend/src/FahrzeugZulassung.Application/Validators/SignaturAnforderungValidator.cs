using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Signatur;
using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Validators;

public class SignaturAnforderungValidator : AbstractValidator<SignaturAnforderungRequest>
{
    public SignaturAnforderungValidator()
    {
        RuleFor(x => x.AuftragId).NotEmpty();
        RuleFor(x => x.SigniererEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.SigniererName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SigniererTelefon)
            .Matches(@"^\+49\d{10,11}$")
            .When(x => x.BevorzugteAuthMethode == SignaturAuthMethode.SMS_TAN)
            .WithMessage("Für SMS-TAN wird eine gültige deutsche Mobilnummer benötigt (+49...)");
        RuleFor(x => x.DokumentBytes)
            .NotNull().WithMessage("Ein Dokument zum Signieren ist erforderlich");
    }
}
