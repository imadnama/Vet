using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.g1.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employees;

    public AuthService(IEmployeeRepository employees) => _employees = employees;

    /// <summary>
    /// Looks up the employee by username and verifies the BCrypt-hashed password.
    /// Sets <paramref name="employee"/> on success, null on failure.
    /// </summary>
    public bool Login(string username, string password, out Employee? employee)
    {
        employee = null;
        try
        {
            var emp = _employees.GetByUsername(username);
            if (emp == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(password, emp.PasswordHash))
                return false;

            employee = emp;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates all fields, hashes the password with BCrypt, and persists the employee.
    /// Returns false if validation fails or username/employee number already exists.
    /// </summary>
    public bool RegisterEmployee(Employee employee, string plainPassword)
    {
        try
        {
            if (!EmployeeValidator.ValidateUsername(employee.Username, out _)) return false;
            if (!EmployeeValidator.ValidatePassword(plainPassword, out _)) return false;
            if (!EmployeeValidator.ValidateEmployeeNumber(employee.EmployeeNumber, out _)) return false;
            if (!EmployeeValidator.ValidateEmail(employee.Email, out _)) return false;
            if (!EmployeeValidator.ValidateNationalId(employee.NationalId, out _)) return false;

            if (_employees.UsernameExists(employee.Username)) return false;
            if (_employees.EmployeeNumberExists(employee.EmployeeNumber)) return false;

            // Never store plain text — always hash before saving.
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            _employees.Add(employee);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateUsername(string username, out string error)
        => EmployeeValidator.ValidateUsername(username, out error);

    public bool ValidatePassword(string password, out string error)
        => EmployeeValidator.ValidatePassword(password, out error);

    public bool ValidateEmployeeNumber(string employeeNumber, out string error)
        => EmployeeValidator.ValidateEmployeeNumber(employeeNumber, out error);
}
