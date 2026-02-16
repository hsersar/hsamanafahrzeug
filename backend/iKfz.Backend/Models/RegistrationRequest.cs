namespace iKfz.Backend.Models;

public class RegistrationRequest
{
    public int Id { get; set; }
    public required string UserId { get; set; } // User ID from OAuth/OIDC
    public required string VIN { get; set; }
    public required string RequestedLicensePlate { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public DateTime FirstRegistrationDate { get; set; }
    public RegistrationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ProcessedBy { get; set; } // Admin user who processed
    public DateTime? ProcessedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public enum RegistrationStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3
}
