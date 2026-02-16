using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Signatur
{
    public Guid Id { get; set; }
    public Guid AuftragId { get; set; }
    public Guid? KundeId { get; set; }
    
    // Signatur-Metadaten
    public SignaturTyp Typ { get; set; }
    public SignaturStatus Status { get; set; } = SignaturStatus.Angefordert;
    public SignaturLevel Level { get; set; } = SignaturLevel.QES;
    public string? Provider { get; set; }                        // z.B. "D-Trust", "Swisscom"
    
    // Dokument-Referenz
    public string DokumentName { get; set; } = string.Empty;    // Name des zu signierenden Dokuments
    public string DokumentHash { get; set; } = string.Empty;    // SHA-256 Hash des Originaldokuments
    public string DokumentHashAlgorithmus { get; set; } = "SHA-256";
    public string? OriginalDokumentPfad { get; set; }            // Pfad zum unsignierten Dokument
    public string? SigniertesDokumentPfad { get; set; }          // Pfad zum signierten Dokument (PDF)
    
    // Provider-Referenzen
    public string? ProviderSessionId { get; set; }               // Session-ID beim QES-Provider
    public string? ProviderTransaktionId { get; set; }           // Transaktions-ID
    public string? ProviderRedirectUrl { get; set; }             // Redirect-URL für Kunden-Auth
    public string? ProviderCallbackUrl { get; set; }             // Callback URL
    
    // Signatur-Ergebnis
    public string? ZertifikatSubject { get; set; }               // CN des Signatur-Zertifikats
    public string? ZertifikatIssuer { get; set; }                // Aussteller (TSP)
    public string? ZertifikatSeriennummer { get; set; }
    public DateTime? ZertifikatGueltigVon { get; set; }
    public DateTime? ZertifikatGueltigBis { get; set; }
    public string? SignaturWert { get; set; }                    // Base64-encoded Signaturwert
    
    // Authentifizierung des Signierers
    public SignaturAuthMethode? AuthMethode { get; set; }        // SMS-TAN, App, Video-Ident
    public string? SigniererName { get; set; }                   // Name aus dem Zertifikat
    public string? SigniererEmail { get; set; }
    
    // Timestamps
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? AngefordertAm { get; set; }
    public DateTime? SigniertAm { get; set; }
    public DateTime? AbgelaufenAm { get; set; }
    public DateTime? AbgelehnAm { get; set; }
    
    // Audit
    public string? FehlerNachricht { get; set; }
    public string? ProviderAntwortJson { get; set; }
    public int Versuche { get; set; } = 0;
    public int MaxVersuche { get; set; } = 3;
    
    // Navigation
    public Auftrag Auftrag { get; set; } = null!;
    public Kunde? Kunde { get; set; }
}
