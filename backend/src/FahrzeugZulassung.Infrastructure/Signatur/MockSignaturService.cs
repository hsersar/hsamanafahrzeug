using System.Security.Cryptography;
using System.Text;
using FahrzeugZulassung.Application.DTOs.Signatur;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FahrzeugZulassung.Infrastructure.Signatur;

/// <summary>
/// Mock-Implementation des QES-Signatur-Services für Entwicklung und Tests.
/// Simuliert den kompletten QES-Flow ohne echte Provider-Anbindung.
/// </summary>
public class MockSignaturService : ISignaturService
{
    private readonly ILogger<MockSignaturService> _logger;
    private readonly Dictionary<Guid, MockSignaturSession> _sessions = new();
    private readonly string _callbackBaseUrl;

    public MockSignaturService(ILogger<MockSignaturService> logger, string callbackBaseUrl = "http://localhost:5000")
    {
        _logger = logger;
        _callbackBaseUrl = callbackBaseUrl;
    }

    public async Task<SignaturAnforderungResult> SignaturAnfordernAsync(SignaturAnforderungRequest request)
    {
        _logger.LogInformation("Mock: Signatur-Anforderung für Auftrag {AuftragId}", request.AuftragId);

        var signaturId = Guid.NewGuid();
        var sessionId = $"MOCK-{Guid.NewGuid():N}";
        
        // Simuliere Dokument-Hash Berechnung
        var documentHash = request.DokumentBytes != null 
            ? ComputeHash(request.DokumentBytes) 
            : "MOCK-HASH-" + Guid.NewGuid().ToString("N");

        var session = new MockSignaturSession
        {
            SignaturId = signaturId,
            SessionId = sessionId,
            AuftragId = request.AuftragId,
            SigniererEmail = request.SigniererEmail,
            SigniererName = request.SigniererName,
            DokumentName = request.DokumentName ?? "Dokument.pdf",
            DokumentHash = documentHash,
            DokumentBytes = request.DokumentBytes,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Status = SignaturStatus.SessionErstellt,
            AuthMethode = request.BevorzugteAuthMethode
        };

        _sessions[signaturId] = session;

        // Simuliere Redirect-URL zum Mock-Provider
        var redirectUrl = $"{_callbackBaseUrl}/api/signatur/{signaturId}/redirect?token={sessionId}";

        _logger.LogInformation("Mock: Session {SessionId} erstellt für Signatur {SignaturId}", sessionId, signaturId);

        return await Task.FromResult(new SignaturAnforderungResult
        {
            Erfolg = true,
            SignaturId = signaturId,
            SessionId = sessionId,
            RedirectUrl = redirectUrl,
            SessionGueltigSekunden = 900 // 15 Minuten
        });
    }

    public async Task<SignaturStatusResult> StatusPruefenAsync(Guid signaturId)
    {
        _logger.LogInformation("Mock: Status prüfen für Signatur {SignaturId}", signaturId);

        if (!_sessions.TryGetValue(signaturId, out var session))
        {
            return await Task.FromResult(new SignaturStatusResult
            {
                SignaturId = signaturId,
                Status = SignaturStatus.Fehlgeschlagen,
                StatusBeschreibung = "Session nicht gefunden",
                FehlerNachricht = "Die angeforderte Signatur-Session existiert nicht."
            });
        }

        // Prüfe ob Session abgelaufen ist
        if (DateTime.UtcNow > session.ExpiresAt && session.Status != SignaturStatus.Signiert)
        {
            session.Status = SignaturStatus.Abgelaufen;
        }

        return await Task.FromResult(new SignaturStatusResult
        {
            SignaturId = signaturId,
            Status = session.Status,
            StatusBeschreibung = GetStatusBeschreibung(session.Status),
            SigniererName = session.SigniererName,
            SigniertAm = session.SigniertAm,
            ZertifikatInfo = session.Status == SignaturStatus.Signiert 
                ? $"CN={session.SigniererName}, O=Mock Trust Services, C=DE" 
                : null,
            DokumentVerfuegbar = session.SignedDocumentBytes != null
        });
    }

    public async Task<SignaturCallbackResult> CallbackVerarbeitenAsync(SignaturCallbackRequest request)
    {
        _logger.LogInformation("Mock: Callback verarbeiten für Signatur {SignaturId}", request.SignaturId);

        if (!_sessions.TryGetValue(request.SignaturId, out var session))
        {
            return await Task.FromResult(new SignaturCallbackResult
            {
                Erfolg = false,
                SignaturId = request.SignaturId,
                FehlerNachricht = "Session nicht gefunden"
            });
        }

        // Simuliere erfolgreiche Signatur
        session.Status = SignaturStatus.Signiert;
        session.SigniertAm = DateTime.UtcNow;
        
        // Erstelle simuliertes signiertes Dokument
        if (session.DokumentBytes != null)
        {
            session.SignedDocumentBytes = CreateMockSignedDocument(session.DokumentBytes, session.SigniererName);
        }

        _logger.LogInformation("Mock: Signatur {SignaturId} erfolgreich abgeschlossen", request.SignaturId);

        return await Task.FromResult(new SignaturCallbackResult
        {
            Erfolg = true,
            SignaturId = request.SignaturId,
            Status = "Signiert"
        });
    }

    public async Task<byte[]> SigniertesDokumentAbrufenAsync(Guid signaturId)
    {
        _logger.LogInformation("Mock: Signiertes Dokument abrufen für Signatur {SignaturId}", signaturId);

        if (!_sessions.TryGetValue(signaturId, out var session))
        {
            throw new InvalidOperationException("Session nicht gefunden");
        }

        if (session.Status != SignaturStatus.Signiert)
        {
            throw new InvalidOperationException($"Dokument kann nicht abgerufen werden. Status: {session.Status}");
        }

        if (session.SignedDocumentBytes == null)
        {
            throw new InvalidOperationException("Signiertes Dokument nicht verfügbar");
        }

        return await Task.FromResult(session.SignedDocumentBytes);
    }

    public async Task<SignaturValidierungResult> SignaturValidierenAsync(Guid signaturId)
    {
        _logger.LogInformation("Mock: Signatur validieren {SignaturId}", signaturId);

        if (!_sessions.TryGetValue(signaturId, out var session))
        {
            return await Task.FromResult(new SignaturValidierungResult
            {
                IstGueltig = false,
                Fehler = new List<string> { "Session nicht gefunden" }
            });
        }

        if (session.Status != SignaturStatus.Signiert)
        {
            return await Task.FromResult(new SignaturValidierungResult
            {
                IstGueltig = false,
                Fehler = new List<string> { $"Signatur ist nicht im Status 'Signiert' (aktuell: {session.Status})" }
            });
        }

        // Mock: Alle Validierungen erfolgreich
        return await Task.FromResult(new SignaturValidierungResult
        {
            IstGueltig = true,
            ZertifikatGueltig = true,
            ZeitstempelGueltig = true,
            DokumentUnveraendert = true,
            SigniererName = session.SigniererName,
            ZertifikatAussteller = "Mock Trust Services GmbH",
            SigniertAm = session.SigniertAm,
            Warnungen = new List<string> { "Dies ist eine Mock-Signatur nur für Entwicklungszwecke" }
        });
    }

    public async Task<bool> StornierenAsync(Guid signaturId)
    {
        _logger.LogInformation("Mock: Signatur stornieren {SignaturId}", signaturId);

        if (!_sessions.TryGetValue(signaturId, out var session))
        {
            return await Task.FromResult(false);
        }

        session.Status = SignaturStatus.Storniert;
        return await Task.FromResult(true);
    }

    private static string ComputeHash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(data);
        return Convert.ToBase64String(hashBytes);
    }

    private static byte[] CreateMockSignedDocument(byte[] originalDocument, string signerName)
    {
        // Für Mock: Füge einfach eine Signatur-Markierung hinzu
        var signatureMarker = Encoding.UTF8.GetBytes(
            $"\n\n--- MOCK QES SIGNATUR ---\n" +
            $"Signiert von: {signerName}\n" +
            $"Datum: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
            $"Zertifikat: CN={signerName}, O=Mock Trust Services, C=DE\n" +
            $"--- ENDE SIGNATUR ---\n"
        );

        var result = new byte[originalDocument.Length + signatureMarker.Length];
        Buffer.BlockCopy(originalDocument, 0, result, 0, originalDocument.Length);
        Buffer.BlockCopy(signatureMarker, 0, result, originalDocument.Length, signatureMarker.Length);
        return result;
    }

    private static string GetStatusBeschreibung(SignaturStatus status) => status switch
    {
        SignaturStatus.Angefordert => "Signatur wurde angefordert",
        SignaturStatus.SessionErstellt => "Provider-Session wurde erstellt",
        SignaturStatus.WartAufAuth => "Warte auf Kunden-Authentifizierung",
        SignaturStatus.AuthErfolgreich => "Kunde hat sich erfolgreich authentifiziert",
        SignaturStatus.InSignierung => "Dokument wird signiert",
        SignaturStatus.Signiert => "Erfolgreich signiert",
        SignaturStatus.Fehlgeschlagen => "Signatur fehlgeschlagen",
        SignaturStatus.Abgelaufen => "Session ist abgelaufen",
        SignaturStatus.Abgelehnt => "Kunde hat die Signatur abgelehnt",
        SignaturStatus.Storniert => "Signatur wurde storniert",
        _ => "Unbekannter Status"
    };

    private class MockSignaturSession
    {
        public Guid SignaturId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public Guid AuftragId { get; set; }
        public string SigniererEmail { get; set; } = string.Empty;
        public string SigniererName { get; set; } = string.Empty;
        public string DokumentName { get; set; } = string.Empty;
        public string DokumentHash { get; set; } = string.Empty;
        public byte[]? DokumentBytes { get; set; }
        public byte[]? SignedDocumentBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? SigniertAm { get; set; }
        public SignaturStatus Status { get; set; }
        public SignaturAuthMethode AuthMethode { get; set; }
    }
}
