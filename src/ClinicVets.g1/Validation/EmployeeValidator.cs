namespace ClinicVets.g1.Validation;

/// <summary>
/// Pure static validation for employee registration fields.
/// All methods return true on success; set <paramref name="error"/> to a user-friendly
/// Hebrew or English message on failure.
/// </summary>
public static class EmployeeValidator
{
    // Rules: 6-8 chars, at most 2 digits, all other chars must be English letters.
    public static bool ValidateUsername(string username, out string error)
        => throw new NotImplementedException();

    // Rules: 8-10 chars, at least 1 letter, 1 digit, 1 special char from {!, #, $}.
    public static bool ValidatePassword(string password, out string error)
        => throw new NotImplementedException();

    // Rules: exactly 4 digits.
    public static bool ValidateEmployeeNumber(string employeeNumber, out string error)
        => throw new NotImplementedException();

    // Rules: must contain '@' and a valid domain part.
    public static bool ValidateEmail(string email, out string error)
        => throw new NotImplementedException();

    // Rules: exactly 9 digits (Israeli national ID format check).
    public static bool ValidateNationalId(string nationalId, out string error)
        => throw new NotImplementedException();
}
