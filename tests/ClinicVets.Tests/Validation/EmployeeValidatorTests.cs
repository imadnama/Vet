using ClinicVets.g1.Validation;
using Xunit;

namespace ClinicVets.Tests.Validation;

/// <summary>
/// Tests for EmployeeValidator.
/// These are pure static methods with no dependencies — no mocks needed.
/// Every rule documented in the source is exercised here.
/// </summary>
public class EmployeeValidatorTests
{
    // ── Full Name ─────────────────────────────────────────────────────────────
    // Rule: letters and spaces only; no digits, no punctuation.

    [Fact]
    public void FullName_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidateFullName("", out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void FullName_WhitespaceOnly_Fails()
    {
        Assert.False(EmployeeValidator.ValidateFullName("   ", out _));
    }

    [Fact]
    public void FullName_LettersAndSpaces_Passes()
    {
        Assert.True(EmployeeValidator.ValidateFullName("John Doe", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void FullName_ContainsDigit_Fails()
    {
        Assert.False(EmployeeValidator.ValidateFullName("John2 Doe", out _));
    }

    [Fact]
    public void FullName_ContainsDash_Fails()
    {
        Assert.False(EmployeeValidator.ValidateFullName("John-Doe", out _));
    }

    // ── Username ──────────────────────────────────────────────────────────────
    // Rules: 6-8 chars; only English letters and digits; at most 2 digits.

    [Fact]
    public void Username_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidateUsername("", out _));
    }

    [Fact]
    public void Username_FiveChars_TooShort_Fails()
    {
        Assert.False(EmployeeValidator.ValidateUsername("abcde", out _));
    }

    [Fact]
    public void Username_NineChars_TooLong_Fails()
    {
        Assert.False(EmployeeValidator.ValidateUsername("abcdefghi", out _));
    }

    [Fact]
    public void Username_SixLetters_Passes()
    {
        Assert.True(EmployeeValidator.ValidateUsername("abcdef", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Username_EightLetters_Passes()
    {
        Assert.True(EmployeeValidator.ValidateUsername("abcdefgh", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Username_TwoDigits_Passes()
    {
        // "abc12d" — 6 chars, exactly 2 digits — right at the allowed limit
        Assert.True(EmployeeValidator.ValidateUsername("abc12d", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Username_ThreeDigits_Fails()
    {
        // "ab123c" — 6 chars but 3 digits — exceeds the 2-digit cap
        Assert.False(EmployeeValidator.ValidateUsername("ab123c", out var error));
        Assert.Contains("2", error);
    }

    [Fact]
    public void Username_SpecialCharacter_Fails()
    {
        Assert.False(EmployeeValidator.ValidateUsername("abc!ef", out _));
    }

    [Fact]
    public void Username_NonEnglishLetter_Fails()
    {
        // Accented letters are not ASCII letters
        Assert.False(EmployeeValidator.ValidateUsername("abcéef", out _));
    }

    // ── Password ──────────────────────────────────────────────────────────────
    // Rules: 8-10 chars; at least 1 letter, 1 digit, 1 special char from {!, #, $}.

    [Fact]
    public void Password_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidatePassword("", out _));
    }

    [Fact]
    public void Password_SevenChars_TooShort_Fails()
    {
        Assert.False(EmployeeValidator.ValidatePassword("Abc1!xy", out _));
    }

    [Fact]
    public void Password_ElevenChars_TooLong_Fails()
    {
        Assert.False(EmployeeValidator.ValidatePassword("Abcdefg1!xy", out _));
    }

    [Fact]
    public void Password_NoLetter_Fails()
    {
        // "1234567!" — 8 chars with digit and special but no letter
        Assert.False(EmployeeValidator.ValidatePassword("1234567!", out var error));
        Assert.Contains("letter", error);
    }

    [Fact]
    public void Password_NoDigit_Fails()
    {
        // "Abcdefg!" — 8 chars with letter and special but no digit
        Assert.False(EmployeeValidator.ValidatePassword("Abcdefg!", out var error));
        Assert.Contains("digit", error);
    }

    [Fact]
    public void Password_NoSpecialChar_Fails()
    {
        // "Abcdefg1" — 8 chars with letter and digit but no special character
        Assert.False(EmployeeValidator.ValidatePassword("Abcdefg1", out var error));
        Assert.Contains("special", error);
    }

    [Fact]
    public void Password_AllRulesMet_WithExclamation_Passes()
    {
        Assert.True(EmployeeValidator.ValidatePassword("Abc1234!", out var error));
        Assert.Empty(error);
    }

    [Theory]
    [InlineData("Abc1234#")]  // hash sign allowed
    [InlineData("Abc1234$")]  // dollar sign allowed
    public void Password_OtherAllowedSpecialChars_Pass(string password)
    {
        Assert.True(EmployeeValidator.ValidatePassword(password, out _));
    }

    // ── Employee Number ───────────────────────────────────────────────────────
    // Rule: exactly 4 digits.

    [Fact]
    public void EmployeeNumber_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmployeeNumber("", out _));
    }

    [Fact]
    public void EmployeeNumber_ThreeDigits_TooShort_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmployeeNumber("123", out var error));
        Assert.Contains("4", error);
    }

    [Fact]
    public void EmployeeNumber_FiveDigits_TooLong_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmployeeNumber("12345", out _));
    }

    [Fact]
    public void EmployeeNumber_ContainsLetter_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmployeeNumber("12A4", out _));
    }

    [Fact]
    public void EmployeeNumber_FourDigits_Passes()
    {
        Assert.True(EmployeeValidator.ValidateEmployeeNumber("1234", out var error));
        Assert.Empty(error);
    }

    // ── Email ─────────────────────────────────────────────────────────────────
    // Rule: must be a valid email address format.

    [Fact]
    public void Email_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmail("", out _));
    }

    [Fact]
    public void Email_MissingAtSign_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmail("userexample.com", out _));
    }

    [Fact]
    public void Email_MissingDomain_Fails()
    {
        Assert.False(EmployeeValidator.ValidateEmail("user@", out _));
    }

    [Fact]
    public void Email_ValidAddress_Passes()
    {
        Assert.True(EmployeeValidator.ValidateEmail("user@example.com", out var error));
        Assert.Empty(error);
    }

    // ── National ID ───────────────────────────────────────────────────────────
    // Rule: exactly 9 digits (Israeli national ID format).

    [Fact]
    public void NationalId_Empty_Fails()
    {
        Assert.False(EmployeeValidator.ValidateNationalId("", out _));
    }

    [Fact]
    public void NationalId_EightDigits_TooShort_Fails()
    {
        Assert.False(EmployeeValidator.ValidateNationalId("12345678", out _));
    }

    [Fact]
    public void NationalId_TenDigits_TooLong_Fails()
    {
        Assert.False(EmployeeValidator.ValidateNationalId("1234567890", out _));
    }

    [Fact]
    public void NationalId_ContainsLetter_Fails()
    {
        Assert.False(EmployeeValidator.ValidateNationalId("12345678A", out _));
    }

    [Fact]
    public void NationalId_NineDigits_Passes()
    {
        Assert.True(EmployeeValidator.ValidateNationalId("123456789", out var error));
        Assert.Empty(error);
    }
}
