namespace FahrzeugZulassung.Domain.Enums;

public enum ZahlungsStatus
{
    Ausstehend = 0,
    InBearbeitung = 1,
    Autorisiert = 2,
    Erfolgreich = 3,
    Fehlgeschlagen = 4,
    Storniert = 5,
    Erstattet = 6
}
