using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g2.Services;
using Moq;
using Xunit;

namespace ClinicVets.Tests.Services;

/// <summary>
/// Tests for AnimalService — add, search, and vaccination alert logic.
/// Both repositories are mocked so no database is needed.
/// </summary>
public class AnimalServiceTests
{
    private readonly Mock<IAnimalRepository>   _animalsMock;
    private readonly Mock<ICustomerRepository> _customersMock;
    private readonly AnimalService             _service;

    public AnimalServiceTests()
    {
        _animalsMock   = new Mock<IAnimalRepository>();
        _customersMock = new Mock<ICustomerRepository>();
        _service       = new AnimalService(_animalsMock.Object, _customersMock.Object);
    }

    // ── AddAnimal — validation failures ──────────────────────────────────────

    [Fact]
    public void AddAnimal_NullAnimal_ReturnsFalse()
    {
        var result = _service.AddAnimal(null!, out var error);

        Assert.False(result);
        Assert.NotEmpty(error);
    }

    [Fact]
    public void AddAnimal_InvalidName_ReturnsFalse()
    {
        // Name contains a digit — AnimalValidator rejects it
        var animal = new Animal { Name = "Buddy2", Weight = 15m, BirthDate = DateTime.Today.AddYears(-2), OwnerId = 1 };

        var result = _service.AddAnimal(animal, out var error);

        Assert.False(result);
        Assert.NotEmpty(error);
    }

    [Fact]
    public void AddAnimal_InvalidWeight_ReturnsFalse()
    {
        var animal = new Animal { Name = "Buddy", Weight = 0m, BirthDate = DateTime.Today.AddYears(-2), OwnerId = 1 };

        var result = _service.AddAnimal(animal, out var error);

        Assert.False(result);
        Assert.NotEmpty(error);
    }

    [Fact]
    public void AddAnimal_FutureBirthDate_ReturnsFalse()
    {
        var animal = new Animal { Name = "Buddy", Weight = 15m, BirthDate = DateTime.Today.AddDays(1), OwnerId = 1 };

        var result = _service.AddAnimal(animal, out var error);

        Assert.False(result);
        Assert.Contains("future", error);
    }

    [Fact]
    public void AddAnimal_FutureVaccinationDate_ReturnsFalse()
    {
        // LastVaccinationDate in the future is not allowed
        var animal = new Animal
        {
            Name               = "Buddy",
            Weight             = 15m,
            BirthDate          = DateTime.Today.AddYears(-2),
            OwnerId            = 1,
            LastVaccinationDate = DateTime.Today.AddDays(1)
        };
        SetupOwnerExists(1);

        var result = _service.AddAnimal(animal, out var error);

        Assert.False(result);
        Assert.Contains("vaccination", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddAnimal_OwnerDoesNotExist_ReturnsFalse()
    {
        var animal = new Animal { Name = "Buddy", Weight = 15m, BirthDate = DateTime.Today.AddYears(-2), OwnerId = 999 };
        // Return no customers — owner 999 does not exist
        _customersMock.Setup(r => r.GetAll()).Returns([]);

        var result = _service.AddAnimal(animal, out var error);

        Assert.False(result);
        Assert.Contains("owner", error, StringComparison.OrdinalIgnoreCase);
    }

    // ── AddAnimal — success path ──────────────────────────────────────────────

    [Fact]
    public void AddAnimal_ValidAnimal_ReturnsTrueAndCallsAdd()
    {
        var animal = MakeValidAnimal();
        SetupOwnerExists(animal.OwnerId);
        // Return null so GenerateUniqueChipNumber exits on the first attempt
        _animalsMock.Setup(r => r.GetByChipNumber(It.IsAny<string>())).Returns((Animal?)null);

        var result = _service.AddAnimal(animal, out var error);

        Assert.True(result);
        Assert.Empty(error);
        _animalsMock.Verify(r => r.Add(animal), Times.Once);
    }

    [Fact]
    public void AddAnimal_ValidAnimal_AssignsChipNumber()
    {
        var animal = MakeValidAnimal();
        SetupOwnerExists(animal.OwnerId);
        _animalsMock.Setup(r => r.GetByChipNumber(It.IsAny<string>())).Returns((Animal?)null);

        _service.AddAnimal(animal, out _);

        // The service must generate and assign a chip number before saving
        Assert.False(string.IsNullOrEmpty(animal.ChipNumber));
        Assert.StartsWith("CV-", animal.ChipNumber);
    }

    [Fact]
    public void AddAnimal_TrimsAnimalName()
    {
        var animal = new Animal { Name = "  Buddy  ", Weight = 15m, BirthDate = DateTime.Today.AddYears(-2), OwnerId = 1 };
        SetupOwnerExists(1);
        _animalsMock.Setup(r => r.GetByChipNumber(It.IsAny<string>())).Returns((Animal?)null);

        _service.AddAnimal(animal, out _);

        Assert.Equal("Buddy", animal.Name);
    }

    // ── NeedsVaccination ──────────────────────────────────────────────────────

    [Fact]
    public void NeedsVaccination_NoVaccinationOnRecord_ReturnsTrue()
    {
        // An animal with no vaccination date is overdue by definition
        var animal = MakeValidAnimal();
        animal.LastVaccinationDate = null;

        Assert.True(_service.NeedsVaccination(animal));
    }

    [Fact]
    public void NeedsVaccination_VaccinatedTwoMonthsAgo_ReturnsFalse()
    {
        var animal = MakeValidAnimal();
        animal.LastVaccinationDate = DateTime.Today.AddMonths(-2);

        Assert.False(_service.NeedsVaccination(animal));
    }

    [Fact]
    public void NeedsVaccination_VaccinatedExactly12MonthsAgo_ReturnsTrue()
    {
        // The threshold is: more than 12 months ago → overdue; exactly 12 months → overdue
        var animal = MakeValidAnimal();
        animal.LastVaccinationDate = DateTime.Today.AddMonths(-12);

        Assert.True(_service.NeedsVaccination(animal));
    }

    [Fact]
    public void NeedsVaccination_VaccinatedOver12MonthsAgo_ReturnsTrue()
    {
        var animal = MakeValidAnimal();
        animal.LastVaccinationDate = DateTime.Today.AddMonths(-13);

        Assert.True(_service.NeedsVaccination(animal));
    }

    // ── SearchByName ──────────────────────────────────────────────────────────

    [Fact]
    public void SearchByName_EmptyString_ReturnsEmpty_WithoutCallingRepo()
    {
        var result = _service.SearchByName("");

        Assert.Empty(result);
        _animalsMock.Verify(r => r.SearchByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SearchByName_ValidName_DelegatesToRepo()
    {
        var expected = new[] { MakeValidAnimal() };
        _animalsMock.Setup(r => r.SearchByName("Buddy")).Returns(expected);

        var result = _service.SearchByName("Buddy");

        Assert.Equal(expected, result);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Animal MakeValidAnimal() => new()
    {
        Name      = "Buddy",
        Weight    = 15m,
        BirthDate = DateTime.Today.AddYears(-2),
        OwnerId   = 1
    };

    private void SetupOwnerExists(int ownerId)
    {
        var owner = new Customer { Id = ownerId, FullName = "Test Owner" };
        _customersMock.Setup(r => r.GetAll()).Returns([owner]);
    }
}
