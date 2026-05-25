namespace ClinicVets.g1.Validation;

/// <summary>
/// Pure static validation for customer registration fields.
/// </summary>
public static class CustomerValidator
{
    // Rules: letters only (no digits, no punctuation).
    public static bool ValidateFullName(string fullName, out string error)
        => throw new NotImplementedException();

    // Rules: exactly 9 digits.
    public static bool ValidateNationalId(string nationalId, out string error)
        => throw new NotImplementedException();

    // Rules: valid Israeli phone format (e.g. 05X-XXXXXXX or 10 digits starting with 05).
    public static bool ValidatePhone(string phone, out string error)
        => throw new NotImplementedException();

    // Rules: valid email format.
    public static bool ValidateEmail(string email, out string error)
        => throw new NotImplementedException();
}
