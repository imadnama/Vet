using ClinicVets.g1.Validation;
using Xunit;

namespace ClinicVets.Tests.Validation;

/// <summary>
/// Tests for CustomerValidator.
/// Pure static methods — no mocks needed.
/// </summary>
public class CustomerValidatorTests
{
    // ── Full Name ─────────────────────────────────────────────────────────────
    // Rule: letters and spaces only.

    [Fact]
    public void FullName_Empty_Fails()
    {
        Assert.False(CustomerValidator.ValidateFullName("", out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void FullName_LettersAndSpaces_Passes()
    {
        Assert.True(CustomerValidator.ValidateFullName("Jane Smith", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void FullName_ContainsDigit_Fails()
    {
        Assert.False(CustomerValidator.ValidateFullName("Jane2 Smith", out _));
    }

    [Fact]
    public void FullName_ContainsSymbol_Fails()
    {
        Assert.False(CustomerValidator.ValidateFullName("Jane@Smith", out _));
    }

    // ── National ID ───────────────────────────────────────────────────────────
    // Rule: exactly 9 digits.

    [Fact]
    public void NationalId_Empty_Fails()
    {
        Assert.False(CustomerValidator.ValidateNationalId("", out _));
    }

    [Fact]
    public void NationalId_EightDigits_TooShort_Fails()
    {
        Assert.False(CustomerValidator.ValidateNationalId("12345678", out _));
    }

    [Fact]
    public void NationalId_ContainsLetter_Fails()
    {
        Assert.False(CustomerValidator.ValidateNationalId("12345678A", out _));
    }

    [Fact]
    public void NationalId_NineDigits_Passes()
    {
        Assert.True(CustomerValidator.ValidateNationalId("123456789", out var error));
        Assert.Empty(error);
    }

    // ── Phone ─────────────────────────────────────────────────────────────────
    // Rules: 10 digits after stripping dashes/spaces; must start with "05".

    [Fact]
    public void Phone_Empty_Fails()
    {
        Assert.False(CustomerValidator.ValidatePhone("", out _));
    }

    [Fact]
    public void Phone_WrongLength_Fails()
    {
        Assert.False(CustomerValidator.ValidatePhone("0521234", out _));
    }

    [Fact]
    public void Phone_DoesNotStartWith05_Fails()
    {
        Assert.False(CustomerValidator.ValidatePhone("0621234567", out var error));
        Assert.Contains("05", error);
    }

    [Fact]
    public void Phone_TenDigitsStartingWith05_Passes()
    {
        Assert.True(CustomerValidator.ValidatePhone("0521234567", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Phone_WithDashes_Passes()
    {
        // Dashes are stripped before validation — "052-123-4567" → "0521234567"
        Assert.True(CustomerValidator.ValidatePhone("052-123-4567", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Phone_WithSpaces_Passes()
    {
        Assert.True(CustomerValidator.ValidatePhone("052 123 4567", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Phone_ContainsLetter_Fails()
    {
        Assert.False(CustomerValidator.ValidatePhone("052123456A", out _));
    }

    // ── Email ─────────────────────────────────────────────────────────────────
    // Rule: must be a valid email address format.

    [Fact]
    public void Email_Empty_Fails()
    {
        Assert.False(CustomerValidator.ValidateEmail("", out _));
    }

    [Fact]
    public void Email_MissingAtSign_Fails()
    {
        Assert.False(CustomerValidator.ValidateEmail("userexample.com", out _));
    }

    [Fact]
    public void Email_ValidAddress_Passes()
    {
        Assert.True(CustomerValidator.ValidateEmail("jane@clinic.com", out var error));
        Assert.Empty(error);
    }
}
