namespace FahrzeugZulassung.Domain.Enums;

public enum SignaturStatus
{
    Angefordert = 0,          // Signatur wurde angefordert
    SessionErstellt = 1,      // Provider-Session erstellt
    WartAufAuth = 2,          // Warte auf Kunden-Authentifizierung
    AuthErfolgreich = 3,      // Kunde hat sich authentifiziert
    InSignierung = 4,         // Dokument wird signiert
    Signiert = 5,             // Erfolgreich signiert
    Fehlgeschlagen = 6,       // Signatur fehlgeschlagen
    Abgelaufen = 7,           // Session abgelaufen
    Abgelehnt = 8,            // Kunde hat abgelehnt
    Storniert = 9             // Vom Mitarbeiter storniert
}
