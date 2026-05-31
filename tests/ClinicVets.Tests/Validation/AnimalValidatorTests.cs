using ClinicVets.g2.Validation;
using Xunit;

namespace ClinicVets.Tests.Validation;

/// <summary>
/// Tests for AnimalValidator.
/// Pure static methods — no mocks needed.
/// </summary>
public class AnimalValidatorTests
{
    // ── Name ─────────────────────────────────────────────────────────────────
    // Rule: letters only (spaces not allowed for animal names).

    [Fact]
    public void Name_Empty_Fails()
    {
        Assert.False(AnimalValidator.ValidateName("", out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void Name_WhitespaceOnly_Fails()
    {
        Assert.False(AnimalValidator.ValidateName("   ", out _));
    }

    [Fact]
    public void Name_LettersOnly_Passes()
    {
        Assert.True(AnimalValidator.ValidateName("Buddy", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Name_ContainsDigit_Fails()
    {
        Assert.False(AnimalValidator.ValidateName("Buddy2", out _));
    }

    [Fact]
    public void Name_ContainsSpace_Fails()
    {
        // Animal names must be a single word — spaces are not letters
        Assert.False(AnimalValidator.ValidateName("Big Buddy", out _));
    }

    // ── Weight ────────────────────────────────────────────────────────────────
    // Rule: between 0.1 kg and 100 kg inclusive.

    [Fact]
    public void Weight_BelowMinimum_Fails()
    {
        Assert.False(AnimalValidator.ValidateWeight(0.05m, out var error));
        Assert.Contains("0.1", error);
    }

    [Fact]
    public void Weight_AboveMaximum_Fails()
    {
        Assert.False(AnimalValidator.ValidateWeight(101m, out var error));
        Assert.Contains("100", error);
    }

    [Fact]
    public void Weight_AtMinimumBoundary_Passes()
    {
        Assert.True(AnimalValidator.ValidateWeight(0.1m, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Weight_AtMaximumBoundary_Passes()
    {
        Assert.True(AnimalValidator.ValidateWeight(100m, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Weight_TypicalValue_Passes()
    {
        Assert.True(AnimalValidator.ValidateWeight(25m, out var error));
        Assert.Empty(error);
    }

    // ── Birth Date ────────────────────────────────────────────────────────────
    // Rules: not in the future; not before 01/01/2000.

    [Fact]
    public void BirthDate_Tomorrow_Fails()
    {
        var future = DateTime.Today.AddDays(1);
        Assert.False(AnimalValidator.ValidateBirthDate(future, out var error));
        Assert.Contains("future", error);
    }

    [Fact]
    public void BirthDate_Before2000_Fails()
    {
        var tooOld = new DateTime(1999, 12, 31);
        Assert.False(AnimalValidator.ValidateBirthDate(tooOld, out var error));
        Assert.Contains("2000", error);
    }

    [Fact]
    public void BirthDate_Today_Passes()
    {
        Assert.True(AnimalValidator.ValidateBirthDate(DateTime.Today, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void BirthDate_ExactlyJan1_2000_Passes()
    {
        // Lower boundary — 01/01/2000 is allowed
        Assert.True(AnimalValidator.ValidateBirthDate(new DateTime(2000, 1, 1), out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void BirthDate_ThreeYearsAgo_Passes()
    {
        Assert.True(AnimalValidator.ValidateBirthDate(DateTime.Today.AddYears(-3), out var error));
        Assert.Empty(error);
    }

    // ── Owner ID ──────────────────────────────────────────────────────────────
    // Rule: must be a positive integer (repository existence check is done in the service).

    [Fact]
    public void OwnerId_Zero_Fails()
    {
        Assert.False(AnimalValidator.ValidateOwnerId(0, out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void OwnerId_Negative_Fails()
    {
        Assert.False(AnimalValidator.ValidateOwnerId(-1, out _));
    }

    [Fact]
    public void OwnerId_Positive_Passes()
    {
        Assert.True(AnimalValidator.ValidateOwnerId(1, out var error));
        Assert.Empty(error);
    }
}
