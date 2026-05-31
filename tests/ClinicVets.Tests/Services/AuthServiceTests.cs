using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Services;
using Moq;
using Xunit;

namespace ClinicVets.Tests.Services;

/// <summary>
/// Tests for AuthService — login and employee registration logic.
/// IEmployeeRepository is mocked so no database is needed.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IEmployeeRepository> _repoMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _service  = new AuthService(_repoMock.Object);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Login_UnknownUsername_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByUsername("nobody")).Returns((Employee?)null);

        var result = _service.Login("nobody", "Abc1234!", out var employee);

        Assert.False(result);
        Assert.Null(employee);
    }

    [Fact]
    public void Login_WrongPassword_ReturnsFalse()
    {
        // Store a hash for "Abc1234!" but try logging in with a different password
        var storedHash = BCrypt.Net.BCrypt.HashPassword("Abc1234!");
        var emp = new Employee { Username = "jdoe12", PasswordHash = storedHash };
        _repoMock.Setup(r => r.GetByUsername("jdoe12")).Returns(emp);

        var result = _service.Login("jdoe12", "WrongPass1!", out var employee);

        Assert.False(result);
        Assert.Null(employee);
    }

    [Fact]
    public void Login_CorrectCredentials_ReturnsTrueAndSetsEmployee()
    {
        const string plainPassword = "Abc1234!";
        var storedHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        var emp = new Employee { Username = "jdoe12", PasswordHash = storedHash };
        _repoMock.Setup(r => r.GetByUsername("jdoe12")).Returns(emp);

        var result = _service.Login("jdoe12", plainPassword, out var employee);

        Assert.True(result);
        Assert.NotNull(employee);
        Assert.Equal("jdoe12", employee.Username);
    }

    // ── RegisterEmployee ──────────────────────────────────────────────────────

    [Fact]
    public void RegisterEmployee_InvalidUsername_ReturnsFalse_WithoutCallingRepo()
    {
        // "abc" is too short — validation should reject before touching the repo
        var emp = MakeValidEmployee();
        emp.Username = "abc";

        var result = _service.RegisterEmployee(emp, "Abc1234!");

        Assert.False(result);
        _repoMock.Verify(r => r.Add(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public void RegisterEmployee_InvalidPassword_ReturnsFalse_WithoutCallingRepo()
    {
        // No special character — password validation should reject it
        var emp = MakeValidEmployee();

        var result = _service.RegisterEmployee(emp, "Abcdefg1");  // missing special char

        Assert.False(result);
        _repoMock.Verify(r => r.Add(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public void RegisterEmployee_DuplicateUsername_ReturnsFalse()
    {
        var emp = MakeValidEmployee();
        _repoMock.Setup(r => r.UsernameExists(emp.Username)).Returns(true);

        var result = _service.RegisterEmployee(emp, "Abc1234!");

        Assert.False(result);
        _repoMock.Verify(r => r.Add(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public void RegisterEmployee_DuplicateEmployeeNumber_ReturnsFalse()
    {
        var emp = MakeValidEmployee();
        _repoMock.Setup(r => r.UsernameExists(emp.Username)).Returns(false);
        _repoMock.Setup(r => r.EmployeeNumberExists(emp.EmployeeNumber)).Returns(true);

        var result = _service.RegisterEmployee(emp, "Abc1234!");

        Assert.False(result);
        _repoMock.Verify(r => r.Add(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public void RegisterEmployee_ValidData_ReturnsTrueAndCallsAdd()
    {
        var emp = MakeValidEmployee();
        _repoMock.Setup(r => r.UsernameExists(emp.Username)).Returns(false);
        _repoMock.Setup(r => r.EmployeeNumberExists(emp.EmployeeNumber)).Returns(false);

        var result = _service.RegisterEmployee(emp, "Abc1234!");

        Assert.True(result);
        _repoMock.Verify(r => r.Add(emp), Times.Once);
    }

    [Fact]
    public void RegisterEmployee_ValidData_StoresBcryptHash_NotPlainPassword()
    {
        const string plainPassword = "Abc1234!";
        var emp = MakeValidEmployee();
        _repoMock.Setup(r => r.UsernameExists(emp.Username)).Returns(false);
        _repoMock.Setup(r => r.EmployeeNumberExists(emp.EmployeeNumber)).Returns(false);

        _service.RegisterEmployee(emp, plainPassword);

        // The service must hash the password before saving — never store plain text
        Assert.NotEqual(plainPassword, emp.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, emp.PasswordHash));
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static Employee MakeValidEmployee() => new()
    {
        FullName       = "John Doe",
        Username       = "jdoe12",
        EmployeeNumber = "1234",
        Email          = "john@clinic.com",
        NationalId     = "123456789"
    };
}
