using FahrzeugZulassung.Application.DTOs;

namespace FahrzeugZulassung.Application.Interfaces;

public interface ITrackingService
{
    Task<TrackingInfo> ErstelleTrackingAsync(Guid auftragId);
    Task<TrackingStatusResponse?> GetStatusByTokenAsync(string trackingCode, string token);
    Task<byte[]> GeneriereQRCodeAsync(string trackingUrl);
    string GeneriereTrackingCode();
    string GeneriereTrackingToken();
}
