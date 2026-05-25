using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employees;

    public AuthService(IEmployeeRepository employees) => _employees = employees;

    /// <summary>
    /// Looks up the employee by username, then verifies the hashed password.
    /// Sets <paramref name="employee"/> on success or null on failure.
    /// </summary>
    public bool Login(string username, string password, out Employee? employee)
        => throw new NotImplementedException();

    /// <summary>
    /// Validates all fields, hashes the password, and persists the new employee.
    /// Returns false (with a specific error logged) if any field is invalid or username/number already exists.
    /// </summary>
    public bool RegisterEmployee(Employee employee, string plainPassword)
        => throw new NotImplementedException();

    public bool ValidateUsername(string username, out string error)
        => throw new NotImplementedException();

    public bool ValidatePassword(string password, out string error)
        => throw new NotImplementedException();

    public bool ValidateEmployeeNumber(string employeeNumber, out string error)
        => throw new NotImplementedException();
}
