using ClinicVets.Core.Enums;

namespace ClinicVets.Core.Models;

public class Employee
{
    public int Id { get; set; }

    /// <summary>6-8 chars, max 2 digits, rest English letters.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Stored as BCrypt hash — never plain text.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Exactly 4 digits.</summary>
    public string EmployeeNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Role Role { get; set; }
}
