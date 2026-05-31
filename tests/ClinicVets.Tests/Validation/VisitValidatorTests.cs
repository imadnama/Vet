using ClinicVets.g3.Validation;
using Xunit;

namespace ClinicVets.Tests.Validation;

/// <summary>
/// Tests for VisitValidator.
/// Pure static methods — no mocks needed.
/// </summary>
public class VisitValidatorTests
{
    // ── Reason ────────────────────────────────────────────────────────────────
    // Rules: not empty; no longer than 500 characters.

    [Fact]
    public void Reason_Empty_Fails()
    {
        Assert.False(VisitValidator.ValidateReason("", out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void Reason_WhitespaceOnly_Fails()
    {
        Assert.False(VisitValidator.ValidateReason("   ", out _));
    }

    [Fact]
    public void Reason_501Chars_TooLong_Fails()
    {
        var tooLong = new string('x', 501);
        Assert.False(VisitValidator.ValidateReason(tooLong, out var error));
        Assert.Contains("500", error);
    }

    [Fact]
    public void Reason_500Chars_AtLimit_Passes()
    {
        var atLimit = new string('x', 500);
        Assert.True(VisitValidator.ValidateReason(atLimit, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Reason_NormalText_Passes()
    {
        Assert.True(VisitValidator.ValidateReason("Annual checkup", out var error));
        Assert.Empty(error);
    }

    // ── Date/Time ─────────────────────────────────────────────────────────────
    // Rule: cannot be in the future (1-minute clock-skew tolerance is built in).

    [Fact]
    public void DateTime_FiveMinutesInFuture_Fails()
    {
        // 5 minutes clearly exceeds the 1-minute tolerance
        var future = System.DateTime.Now.AddMinutes(5);
        Assert.False(VisitValidator.ValidateDateTime(future, out var error));
        Assert.Contains("future", error);
    }

    [Fact]
    public void DateTime_Now_Passes()
    {
        Assert.True(VisitValidator.ValidateDateTime(System.DateTime.Now, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void DateTime_OneHourAgo_Passes()
    {
        var pastHour = System.DateTime.Now.AddHours(-1);
        Assert.True(VisitValidator.ValidateDateTime(pastHour, out var error));
        Assert.Empty(error);
    }

    // ── Animal ID ─────────────────────────────────────────────────────────────
    // Rule: must be a positive integer (means an animal was actually selected).

    [Fact]
    public void AnimalId_Zero_Fails()
    {
        Assert.False(VisitValidator.ValidateAnimalId(0, out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void AnimalId_Negative_Fails()
    {
        Assert.False(VisitValidator.ValidateAnimalId(-5, out _));
    }

    [Fact]
    public void AnimalId_Positive_Passes()
    {
        Assert.True(VisitValidator.ValidateAnimalId(1, out var error));
        Assert.Empty(error);
    }

    // ── Diagnosis ─────────────────────────────────────────────────────────────
    // Rules: optional (empty is fine); no longer than 1000 characters.

    [Fact]
    public void Diagnosis_Empty_Passes()
    {
        // Diagnosis is optional — empty string is valid
        Assert.True(VisitValidator.ValidateDiagnosis("", out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Diagnosis_1001Chars_TooLong_Fails()
    {
        var tooLong = new string('d', 1001);
        Assert.False(VisitValidator.ValidateDiagnosis(tooLong, out var error));
        Assert.Contains("1000", error);
    }

    [Fact]
    public void Diagnosis_1000Chars_AtLimit_Passes()
    {
        var atLimit = new string('d', 1000);
        Assert.True(VisitValidator.ValidateDiagnosis(atLimit, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Diagnosis_NormalText_Passes()
    {
        Assert.True(VisitValidator.ValidateDiagnosis("Minor ear infection, prescribed antibiotics", out var error));
        Assert.Empty(error);
    }
}
