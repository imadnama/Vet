using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

/// <summary>Implemented by g1 — AuthService.</summary>
public interface IAuthService
{
    /// <summary>Checks username/password and returns the matched employee on success.</summary>
    bool Login(string username, string password, out Employee? employee);

    bool RegisterEmployee(Employee employee, string plainPassword);

    // Validation helpers exposed so the form can give inline feedback
    bool ValidateUsername(string username, out string error);
    bool ValidatePassword(string password, out string error);
    bool ValidateEmployeeNumber(string employeeNumber, out string error);
}
