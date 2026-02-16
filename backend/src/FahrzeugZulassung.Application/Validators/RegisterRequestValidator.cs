using FluentValidation;
using FahrzeugZulassung.Application.DTOs.Auth;
using System.Text.RegularExpressions;

namespace FahrzeugZulassung.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
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
            .WithMessage("Passwort ist erforderlich")
            .MinimumLength(12)
            .WithMessage("Passwort muss mindestens 12 Zeichen lang sein (BSI-Anforderung)")
            .Must(ContainUpperCase)
            .WithMessage("Passwort muss mindestens einen Großbuchstaben enthalten")
            .Must(ContainLowerCase)
            .WithMessage("Passwort muss mindestens einen Kleinbuchstaben enthalten")
            .Must(ContainDigit)
            .WithMessage("Passwort muss mindestens eine Ziffer enthalten")
            .Must(ContainSpecialCharacter)
            .WithMessage("Passwort muss mindestens ein Sonderzeichen enthalten");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Passwortbestätigung ist erforderlich")
            .Equal(x => x.Password)
            .WithMessage("Passwörter stimmen nicht überein");

        RuleFor(x => x.Vorname)
            .NotEmpty()
            .WithMessage("Vorname ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Vorname darf maximal 100 Zeichen lang sein");

        RuleFor(x => x.Nachname)
            .NotEmpty()
            .WithMessage("Nachname ist erforderlich")
            .MaximumLength(100)
            .WithMessage("Nachname darf maximal 100 Zeichen lang sein");

        RuleFor(x => x.Telefon)
            .MaximumLength(20)
            .WithMessage("Telefonnummer darf maximal 20 Zeichen lang sein")
            .Matches(@"^[\d\s\+\-\(\)\/]+$")
            .WithMessage("Ungültige Telefonnummer")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefon));
    }

    private bool ContainUpperCase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
    }

    private bool ContainLowerCase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsLower);
    }

    private bool ContainDigit(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);
    }

    private bool ContainSpecialCharacter(string password)
    {
        return !string.IsNullOrEmpty(password) && 
               Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
    }
}
