using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Auth;

namespace FahrzeugZulassung.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email ist erforderlich")
            .EmailAddress()
            .WithMessage("Ungültige Email-Adresse")
            .MaximumLength(255)
            .WithMessage("Email darf maximal 255 Zeichen lang sein");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Passwort ist erforderlich");
    }
}
