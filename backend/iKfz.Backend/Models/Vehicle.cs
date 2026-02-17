namespace iKfz.Backend.Models;

public class Vehicle
{
    public int Id { get; set; }
    public required string VIN { get; set; } // Vehicle Identification Number
    public required string LicensePlate { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public DateTime FirstRegistrationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property for registration
    public required string OwnerId { get; set; } // User ID from OAuth/OIDC
}
