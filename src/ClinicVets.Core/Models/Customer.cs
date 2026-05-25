namespace ClinicVets.Core.Models;

public class Customer
{
    public int Id { get; set; }

    /// <summary>Letters only.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Exactly 9 digits.</summary>
    public string NationalId { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
