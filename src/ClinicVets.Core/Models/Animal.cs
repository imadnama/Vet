using ClinicVets.Core.Enums;

namespace ClinicVets.Core.Models;

public class Animal
{
    public int Id { get; set; }

    /// <summary>Auto-generated serial number (chip number).</summary>
    public string ChipNumber { get; set; } = string.Empty;

    /// <summary>Letters only.</summary>
    public string Name { get; set; } = string.Empty;

    public AnimalType Type { get; set; }

    /// <summary>Positive decimal between 0.1 and 100 kg.</summary>
    public decimal Weight { get; set; }

    /// <summary>Not in the future and not before year 2000.</summary>
    public DateTime BirthDate { get; set; }

    /// <summary>FK to Customer.Id — required, must exist.</summary>
    public int OwnerId { get; set; }

    public DateTime? LastVaccinationDate { get; set; }
}
