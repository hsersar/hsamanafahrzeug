using FahrzeugZulassung.Infrastructure.Services;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace FahrzeugZulassung.Tests;

public class TrackingServiceTests
{
    [Fact]
    public void GeneriereTrackingCode_ShouldReturnValidFormat()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var service = new TrackingService(null!, configuration);

        // Act
        var trackingCode = service.GeneriereTrackingCode();

        // Assert
        Assert.NotNull(trackingCode);
        Assert.StartsWith("TRK-", trackingCode);
        Assert.Equal(23, trackingCode.Length); // TRK-XXXX-XXXX-XXXX-XXXX (19 chars + 4 dashes)
        Assert.Matches(@"^TRK-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$", trackingCode);
    }

    [Fact]
    public void GeneriereTrackingCode_ShouldGenerateUniqueCodes()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var service = new TrackingService(null!, configuration);

        // Act
        var code1 = service.GeneriereTrackingCode();
        var code2 = service.GeneriereTrackingCode();

        // Assert
        Assert.NotEqual(code1, code2);
    }

    [Fact]
    public void GeneriereTrackingToken_ShouldReturn64CharacterHexString()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var service = new TrackingService(null!, configuration);

        // Act
        var token = service.GeneriereTrackingToken();

        // Assert
        Assert.NotNull(token);
        Assert.Equal(64, token.Length);
        Assert.Matches(@"^[a-f0-9]{64}$", token);
    }

    [Fact]
    public void GeneriereTrackingToken_ShouldGenerateUniqueTokens()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var service = new TrackingService(null!, configuration);

        // Act
        var token1 = service.GeneriereTrackingToken();
        var token2 = service.GeneriereTrackingToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }
}

public class DomainEntityTests
{
    [Fact]
    public void Zahlung_ShouldHaveDefaultStatus()
    {
        // Arrange & Act
        var zahlung = new Zahlung();

        // Assert
        Assert.Equal(ZahlungsStatus.Ausstehend, zahlung.Status);
        Assert.Equal("EUR", zahlung.Waehrung);
    }

    [Fact]
    public void AuftragTracking_ShouldInitializeWithZeroAccess()
    {
        // Arrange & Act
        var tracking = new AuftragTracking();

        // Assert
        Assert.Equal(0, tracking.ZugriffAnzahl);
        Assert.False(tracking.EmailGesendet);
        Assert.False(tracking.SMSGesendet);
    }

    [Fact]
    public void Rechnung_ShouldInitializeWithDefaultCurrency()
    {
        // Arrange & Act
        var rechnung = new Rechnung();

        // Assert
        Assert.Equal("EUR", rechnung.Waehrung);
        Assert.False(rechnung.IstBezahlt);
        Assert.Empty(rechnung.Zahlungen);
    }
}

