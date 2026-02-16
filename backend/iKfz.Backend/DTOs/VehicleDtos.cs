using System.ComponentModel.DataAnnotations;

namespace iKfz.Backend.DTOs;

public class CreateRegistrationRequestDto
{
    [Required]
    [StringLength(17, MinimumLength = 17)]
    public required string VIN { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 1)]
    public required string RequestedLicensePlate { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Brand { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Model { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Color { get; set; }

    [Required]
    public DateTime FirstRegistrationDate { get; set; }
}

public class RegistrationRequestDto
{
    public int Id { get; set; }
    public required string VIN { get; set; }
    public required string RequestedLicensePlate { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public DateTime FirstRegistrationDate { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public class VehicleDto
{
    public int Id { get; set; }
    public required string VIN { get; set; }
    public required string LicensePlate { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public DateTime FirstRegistrationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
