using System.Security.Cryptography;
using System.Text;
using FahrzeugZulassung.Application.DTOs;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace FahrzeugZulassung.Infrastructure.Services;

public class TrackingService : ITrackingService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public TrackingService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TrackingInfo> ErstelleTrackingAsync(Guid auftragId)
    {
        var auftrag = await _context.Auftraege
            .Include(a => a.Tracking)
            .FirstOrDefaultAsync(a => a.Id == auftragId);

        if (auftrag == null)
            throw new ArgumentException("Auftrag nicht gefunden", nameof(auftragId));

        // Check if tracking already exists
        if (auftrag.Tracking != null)
        {
            var existingUrl = GetTrackingUrl(auftrag.Tracking.TrackingCode, auftrag.Tracking.TrackingToken);
            var existingQRCode = await GeneriereQRCodeAsync(existingUrl);
            
            return new TrackingInfo
            {
                TrackingCode = auftrag.Tracking.TrackingCode,
                TrackingUrl = existingUrl,
                QRCodeBytes = existingQRCode,
                QRCodeBase64 = Convert.ToBase64String(existingQRCode)
            };
        }

        // Create new tracking
        var trackingCode = GeneriereTrackingCode();
        var trackingToken = GeneriereTrackingToken();
        var trackingUrl = GetTrackingUrl(trackingCode, trackingToken);

        var tracking = new AuftragTracking
        {
            Id = Guid.NewGuid(),
            AuftragId = auftragId,
            TrackingCode = trackingCode,
            TrackingToken = trackingToken,
            QRCodeUrl = trackingUrl
        };

        _context.AuftragTrackings.Add(tracking);
        await _context.SaveChangesAsync();

        var qrCodeBytes = await GeneriereQRCodeAsync(trackingUrl);

        return new TrackingInfo
        {
            TrackingCode = trackingCode,
            TrackingUrl = trackingUrl,
            QRCodeBytes = qrCodeBytes,
            QRCodeBase64 = Convert.ToBase64String(qrCodeBytes)
        };
    }

    public async Task<TrackingStatusResponse?> GetStatusByTokenAsync(string trackingCode, string token)
    {
        var tracking = await _context.AuftragTrackings
            .Include(t => t.Auftrag)
                .ThenInclude(a => a.Rechnung)
                    .ThenInclude(r => r!.Zahlungen)
            .Include(t => t.Auftrag)
                .ThenInclude(a => a.StatusHistorie)
            .FirstOrDefaultAsync(t => t.TrackingCode == trackingCode && t.TrackingToken == token);

        if (tracking == null)
            return null;

        // Update access tracking
        tracking.LetzterZugriffAm = DateTime.UtcNow;
        tracking.ZugriffAnzahl++;
        await _context.SaveChangesAsync();

        var auftrag = tracking.Auftrag;
        var kennzeichenMaskiert = MaskiereKennzeichen(auftrag.Kennzeichen);

        var response = new TrackingStatusResponse
        {
            TrackingCode = trackingCode,
            AuftragTyp = auftrag.AuftragTyp,
            AktuellerStatus = auftrag.Status.ToString(),
            StatusBeschreibung = GetStatusBeschreibung(auftrag.Status),
            FortschrittProzent = GetFortschrittProzent(auftrag.Status),
            ErstelltAm = auftrag.ErstelltAm,
            LetzteAktualisierung = auftrag.LetzteAktualisierung ?? auftrag.ErstelltAm,
            Kennzeichen = kennzeichenMaskiert,
            ZahlungErforderlich = auftrag.Rechnung != null,
            ZahlungErfolgt = auftrag.Rechnung?.IstBezahlt ?? false,
            Historie = auftrag.StatusHistorie
                .OrderBy(h => h.ZeitpunktAm)
                .Select(h => new StatusHistorieEintrag
                {
                    Status = h.NeuerStatus.ToString(),
                    Beschreibung = GetStatusBeschreibung(h.NeuerStatus),
                    Zeitpunkt = h.ZeitpunktAm,
                    IstAktuell = h.NeuerStatus == auftrag.Status
                })
                .ToList()
        };

        return response;
    }

    public async Task<byte[]> GeneriereQRCodeAsync(string trackingUrl)
    {
        return await Task.Run(() =>
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(trackingUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        });
    }

    public string GeneriereTrackingCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = RandomNumberGenerator.GetBytes(16);
        
        var result = new char[16];
        for (int i = 0; i < 16; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }
        
        return $"TRK-{new string(result, 0, 4)}-{new string(result, 4, 4)}-{new string(result, 8, 4)}-{new string(result, 12, 4)}";
    }

    public string GeneriereTrackingToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(tokenBytes).ToLower();
    }

    private string GetTrackingUrl(string trackingCode, string token)
    {
        var baseUrl = _configuration["Tracking:BaseUrl"] ?? "http://localhost:3000/tracking";
        return $"{baseUrl}/{trackingCode}?token={token}";
    }

    private string? MaskiereKennzeichen(string? kennzeichen)
    {
        if (string.IsNullOrEmpty(kennzeichen))
            return null;

        if (kennzeichen.Length <= 4)
            return "***";

        // Show first part and last 4 digits: "B-**1234"
        var parts = kennzeichen.Split('-');
        if (parts.Length == 2)
        {
            return $"{parts[0]}-**{parts[1]}";
        }

        return kennzeichen.Substring(0, 2) + "**" + kennzeichen.Substring(kennzeichen.Length - 4);
    }

    private string GetStatusBeschreibung(AuftragStatus status)
    {
        return status switch
        {
            AuftragStatus.Eingereicht => "Ihr Antrag wurde erfolgreich eingereicht",
            AuftragStatus.InBearbeitung => "Ihr Antrag wird bearbeitet",
            AuftragStatus.WartetAufZahlung => "Bitte führen Sie die Zahlung durch",
            AuftragStatus.AnIKFZGesendet => "Ihr Antrag wurde an iKFZ gesendet",
            AuftragStatus.Genehmigt => "Ihr Antrag wurde genehmigt",
            AuftragStatus.Abgeschlossen => "Ihr Antrag wurde erfolgreich abgeschlossen",
            AuftragStatus.Abgelehnt => "Ihr Antrag wurde abgelehnt",
            AuftragStatus.Storniert => "Ihr Antrag wurde storniert",
            _ => status.ToString()
        };
    }

    private int GetFortschrittProzent(AuftragStatus status)
    {
        return status switch
        {
            AuftragStatus.Eingereicht => 15,
            AuftragStatus.InBearbeitung => 30,
            AuftragStatus.WartetAufZahlung => 45,
            AuftragStatus.AnIKFZGesendet => 60,
            AuftragStatus.Genehmigt => 85,
            AuftragStatus.Abgeschlossen => 100,
            AuftragStatus.Abgelehnt => 0,
            AuftragStatus.Storniert => 0,
            _ => 0
        };
    }
}
