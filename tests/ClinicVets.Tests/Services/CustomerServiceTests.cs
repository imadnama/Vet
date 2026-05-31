using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Services;
using Moq;
using Xunit;

namespace ClinicVets.Tests.Services;

/// <summary>
/// Tests for CustomerService — registration and search logic.
/// Both repositories are mocked so no database is needed.
/// </summary>
public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customersMock;
    private readonly Mock<IAnimalRepository>   _animalsMock;
    private readonly CustomerService           _service;

    public CustomerServiceTests()
    {
        _customersMock = new Mock<ICustomerRepository>();
        _animalsMock   = new Mock<IAnimalRepository>();
        _service       = new CustomerService(_customersMock.Object, _animalsMock.Object);
    }

    // ── RegisterCustomer ──────────────────────────────────────────────────────

    [Fact]
    public void RegisterCustomer_InvalidName_ReturnsFalse_WithError()
    {
        // "Jane2 Smith" contains a digit — validator should reject it
        var customer = new Customer
        {
            FullName   = "Jane2 Smith",
            NationalId = "123456789",
            Phone      = "0521234567",
            Email      = "jane@example.com"
        };

        var result = _service.RegisterCustomer(customer, out var error);

        Assert.False(result);
        Assert.NotEmpty(error);
        _customersMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public void RegisterCustomer_InvalidPhone_ReturnsFalse_WithError()
    {
        // Does not start with "05"
        var customer = new Customer
        {
            FullName   = "Jane Smith",
            NationalId = "123456789",
            Phone      = "0621234567",
            Email      = "jane@example.com"
        };

        var result = _service.RegisterCustomer(customer, out var error);

        Assert.False(result);
        Assert.NotEmpty(error);
    }

    [Fact]
    public void RegisterCustomer_DuplicateNationalId_ReturnsFalse_WithError()
    {
        var customer = MakeValidCustomer();
        _customersMock.Setup(r => r.NationalIdExists(customer.NationalId)).Returns(true);

        var result = _service.RegisterCustomer(customer, out var error);

        Assert.False(result);
        Assert.Contains("already exists", error);
        _customersMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public void RegisterCustomer_ValidData_ReturnsTrueAndCallsAdd()
    {
        var customer = MakeValidCustomer();
        _customersMock.Setup(r => r.NationalIdExists(customer.NationalId)).Returns(false);

        var result = _service.RegisterCustomer(customer, out var error);

        Assert.True(result);
        Assert.Empty(error);
        _customersMock.Verify(r => r.Add(customer), Times.Once);
    }

    [Fact]
    public void RegisterCustomer_NormalizesPhoneBeforeSaving()
    {
        // Phone entered with dashes should be stored as plain digits
        var customer = new Customer
        {
            FullName   = "Jane Smith",
            NationalId = "123456789",
            Phone      = "052-123-4567",
            Email      = "jane@example.com"
        };
        _customersMock.Setup(r => r.NationalIdExists(customer.NationalId)).Returns(false);

        _service.RegisterCustomer(customer, out _);

        Assert.Equal("0521234567", customer.Phone);
    }

    // ── SearchByNationalId ────────────────────────────────────────────────────

    [Fact]
    public void SearchByNationalId_ExistingId_ReturnsCustomer()
    {
        var expected = MakeValidCustomer();
        _customersMock.Setup(r => r.GetByNationalId("123456789")).Returns(expected);

        var result = _service.SearchByNationalId("123456789");

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SearchByNationalId_UnknownId_ReturnsNull()
    {
        _customersMock.Setup(r => r.GetByNationalId("999999999")).Returns((Customer?)null);

        var result = _service.SearchByNationalId("999999999");

        Assert.Null(result);
    }

    // ── SearchByPhone ─────────────────────────────────────────────────────────

    [Fact]
    public void SearchByPhone_NormalizesBeforeQuerying()
    {
        // The service strips dashes before hitting the repo
        var expected = MakeValidCustomer();
        _customersMock.Setup(r => r.GetByPhone("0521234567")).Returns(expected);

        var result = _service.SearchByPhone("052-123-4567");

        Assert.Equal(expected, result);
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static Customer MakeValidCustomer() => new()
    {
        FullName   = "Jane Smith",
        NationalId = "123456789",
        Phone      = "0521234567",
        Email      = "jane@example.com"
    };
}
