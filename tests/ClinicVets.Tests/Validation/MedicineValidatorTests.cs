using ClinicVets.g3.Validation;
using Xunit;

namespace ClinicVets.Tests.Validation;

/// <summary>
/// Tests for MedicineValidator.
/// Pure static methods — no mocks needed.
/// </summary>
public class MedicineValidatorTests
{
    // ── Name ─────────────────────────────────────────────────────────────────
    // Rules: not empty; no longer than 200 characters.

    [Fact]
    public void Name_Empty_Fails()
    {
        Assert.False(MedicineValidator.ValidateName("", out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void Name_WhitespaceOnly_Fails()
    {
        Assert.False(MedicineValidator.ValidateName("   ", out _));
    }

    [Fact]
    public void Name_201Chars_TooLong_Fails()
    {
        var tooLong = new string('A', 201);
        Assert.False(MedicineValidator.ValidateName(tooLong, out var error));
        Assert.Contains("200", error);
    }

    [Fact]
    public void Name_200Chars_AtLimit_Passes()
    {
        var atLimit = new string('A', 200);
        Assert.True(MedicineValidator.ValidateName(atLimit, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Name_NormalMedicineName_Passes()
    {
        Assert.True(MedicineValidator.ValidateName("Amoxicillin 500mg", out var error));
        Assert.Empty(error);
    }

    // ── Price ─────────────────────────────────────────────────────────────────
    // Rules: must be greater than 0; must not exceed 100,000.

    [Fact]
    public void Price_Zero_Fails()
    {
        Assert.False(MedicineValidator.ValidatePrice(0m, out var error));
        Assert.Contains("0", error);
    }

    [Fact]
    public void Price_Negative_Fails()
    {
        Assert.False(MedicineValidator.ValidatePrice(-5m, out _));
    }

    [Fact]
    public void Price_AboveMaximum_Fails()
    {
        Assert.False(MedicineValidator.ValidatePrice(100001m, out _));
    }

    [Fact]
    public void Price_AtMaximumBoundary_Passes()
    {
        Assert.True(MedicineValidator.ValidatePrice(100000m, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Price_TypicalValue_Passes()
    {
        Assert.True(MedicineValidator.ValidatePrice(49.99m, out var error));
        Assert.Empty(error);
    }

    // ── Quantity ──────────────────────────────────────────────────────────────
    // Rules: cannot be negative; must not exceed 10,000.

    [Fact]
    public void Quantity_Negative_Fails()
    {
        Assert.False(MedicineValidator.ValidateQuantity(-1, out var error));
        Assert.Contains("negative", error);
    }

    [Fact]
    public void Quantity_Zero_IsAllowed_Passes()
    {
        // Zero means out-of-stock, which is a valid inventory state
        Assert.True(MedicineValidator.ValidateQuantity(0, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Quantity_AboveMaximum_Fails()
    {
        Assert.False(MedicineValidator.ValidateQuantity(10001, out _));
    }

    [Fact]
    public void Quantity_AtMaximumBoundary_Passes()
    {
        Assert.True(MedicineValidator.ValidateQuantity(10000, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Quantity_TypicalValue_Passes()
    {
        Assert.True(MedicineValidator.ValidateQuantity(50, out var error));
        Assert.Empty(error);
    }
}
