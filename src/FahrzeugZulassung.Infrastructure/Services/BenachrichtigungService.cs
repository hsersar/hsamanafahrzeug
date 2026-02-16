using FahrzeugZulassung.Application.DTOs;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace FahrzeugZulassung.Infrastructure.Services;

public class BenachrichtigungService : IBenachrichtigungService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITrackingService _trackingService;
    private readonly ILogger<BenachrichtigungService> _logger;

    public BenachrichtigungService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ITrackingService trackingService,
        ILogger<BenachrichtigungService> logger)
    {
        _context = context;
        _configuration = configuration;
        _trackingService = trackingService;
        _logger = logger;
    }

    public async Task<bool> SendeTrackingEmailAsync(Guid auftragId, string empfaengerEmail)
    {
        try
        {
            var auftrag = await _context.Auftraege
                .Include(a => a.Tracking)
                .FirstOrDefaultAsync(a => a.Id == auftragId);

            if (auftrag == null)
                return false;

            FahrzeugZulassung.Application.DTOs.TrackingInfo trackingInfo;
            if (auftrag.Tracking == null)
            {
                trackingInfo = await _trackingService.ErstelleTrackingAsync(auftragId);
            }
            else
            {
                var trackingUrl = $"{_configuration["Tracking:BaseUrl"]}/{auftrag.Tracking.TrackingCode}?token={auftrag.Tracking.TrackingToken}";
                var qrCodeBytes = await _trackingService.GeneriereQRCodeAsync(trackingUrl);
                trackingInfo = new FahrzeugZulassung.Application.DTOs.TrackingInfo
                {
                    TrackingCode = auftrag.Tracking.TrackingCode,
                    TrackingUrl = trackingUrl,
                    QRCodeBytes = qrCodeBytes,
                    QRCodeBase64 = Convert.ToBase64String(qrCodeBytes)
                };
            }

            var htmlBody = ErstelleTrackingEmailHtml(auftrag.Antragsnummer, trackingInfo);

            await SendeEmailAsync(
                empfaengerEmail,
                "Ihr KFZ-Zulassungs Tracking-Link",
                htmlBody,
                trackingInfo.QRCodeBytes);

            // Update tracking
            if (auftrag.Tracking != null)
            {
                auftrag.Tracking.EmailGesendet = true;
                auftrag.Tracking.EmailGesendetAm = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Tracking-E-Mail");
            return false;
        }
    }

    public async Task<bool> SendeTrackingSMSAsync(Guid auftragId, string telefonnummer)
    {
        try
        {
            var auftrag = await _context.Auftraege
                .Include(a => a.Tracking)
                .FirstOrDefaultAsync(a => a.Id == auftragId);

            if (auftrag?.Tracking == null)
                return false;

            var trackingUrl = $"{_configuration["Tracking:BaseUrl"]}/{auftrag.Tracking.TrackingCode}?token={auftrag.Tracking.TrackingToken}";
            var message = $"Ihr KFZ-Antrag {auftrag.Tracking.TrackingCode} - Status verfolgen: {trackingUrl}";

            // SMS-Versand würde hier implementiert werden (z.B. über Twilio)
            _logger.LogInformation("SMS würde gesendet an {Telefon}: {Message}", telefonnummer, message);

            auftrag.Tracking.SMSGesendet = true;
            auftrag.Tracking.SMSGesendetAm = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Tracking-SMS");
            return false;
        }
    }

    public async Task<bool> SendeStatusUpdateEmailAsync(Guid auftragId, AuftragStatus neuerStatus)
    {
        try
        {
            var auftrag = await _context.Auftraege
                .Include(a => a.Tracking)
                .FirstOrDefaultAsync(a => a.Id == auftragId);

            if (auftrag == null)
                return false;

            var statusText = GetStatusBeschreibung(neuerStatus);
            var htmlBody = ErstelleStatusUpdateEmailHtml(auftrag.Antragsnummer, statusText);

            await SendeEmailAsync(
                auftrag.KundenEmail,
                $"Status-Update: {auftrag.Antragsnummer}",
                htmlBody);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Status-Update-E-Mail");
            return false;
        }
    }

    public async Task<bool> SendeZahlungsbestaetigungAsync(Guid rechnungId)
    {
        try
        {
            var rechnung = await _context.Rechnungen
                .Include(r => r.Auftrag)
                .FirstOrDefaultAsync(r => r.Id == rechnungId);

            if (rechnung == null)
                return false;

            var htmlBody = ErstelleZahlungsbestaetigungEmailHtml(
                rechnung.Rechnungsnummer,
                rechnung.Betrag,
                rechnung.Waehrung);

            await SendeEmailAsync(
                rechnung.Auftrag.KundenEmail,
                $"Zahlungsbestätigung: {rechnung.Rechnungsnummer}",
                htmlBody);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Zahlungsbestätigung");
            return false;
        }
    }

    private async Task SendeEmailAsync(string empfaenger, string betreff, string htmlBody, byte[]? qrCodeBytes = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["Email:SenderName"] ?? "KFZ-Zulassung",
            _configuration["Email:SenderEmail"] ?? "noreply@example.de"));
        message.To.Add(MailboxAddress.Parse(empfaenger));
        message.Subject = betreff;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };

        if (qrCodeBytes != null)
        {
            bodyBuilder.Attachments.Add("tracking-qrcode.png", qrCodeBytes, new ContentType("image", "png"));
        }

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        
        // In development, just log instead of sending
        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogInformation("E-Mail würde gesendet werden an {Empfaenger}: {Betreff}", empfaenger, betreff);
            return;
        }

        await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        
        var smtpUser = _configuration["Email:SmtpUser"];
        var smtpPassword = _configuration["Email:SmtpPassword"];
        
        if (!string.IsNullOrEmpty(smtpUser))
        {
            await client.AuthenticateAsync(smtpUser, smtpPassword);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private string ErstelleTrackingEmailHtml(string antragsnummer, Application.DTOs.TrackingInfo trackingInfo)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
        .tracking-code {{ font-size: 24px; font-weight: bold; color: #007bff; text-align: center; padding: 20px; }}
        .button {{ display: inline-block; padding: 12px 24px; background: #007bff; color: white; text-decoration: none; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>KFZ-Zulassung Tracking</h1>
        </div>
        <div class=""content"">
            <p>Sehr geehrte Damen und Herren,</p>
            <p>Ihr Antrag <strong>{antragsnummer}</strong> wurde erfolgreich eingereicht.</p>
            <div class=""tracking-code"">
                Tracking-Code: {trackingInfo.TrackingCode}
            </div>
            <p style=""text-align: center;"">
                <a href=""{trackingInfo.TrackingUrl}"" class=""button"">Status verfolgen</a>
            </p>
            <p>Oder scannen Sie den QR-Code im Anhang, um den Status zu verfolgen.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string ErstelleStatusUpdateEmailHtml(string antragsnummer, string statusText)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Status-Update</h1>
        </div>
        <div class=""content"">
            <p>Sehr geehrte Damen und Herren,</p>
            <p>Der Status Ihres Antrags <strong>{antragsnummer}</strong> hat sich geändert:</p>
            <p style=""font-size: 18px; color: #28a745;""><strong>{statusText}</strong></p>
        </div>
    </div>
</body>
</html>";
    }

    private string ErstelleZahlungsbestaetigungEmailHtml(string rechnungsnummer, decimal betrag, string waehrung)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Zahlungsbestätigung</h1>
        </div>
        <div class=""content"">
            <p>Sehr geehrte Damen und Herren,</p>
            <p>Ihre Zahlung für Rechnung <strong>{rechnungsnummer}</strong> wurde erfolgreich empfangen.</p>
            <p>Betrag: <strong>{betrag:F2} {waehrung}</strong></p>
            <p>Vielen Dank!</p>
        </div>
    </div>
</body>
</html>";
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
}
