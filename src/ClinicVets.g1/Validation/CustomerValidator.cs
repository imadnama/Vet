namespace ClinicVets.g1.Validation;

/// <summary>
/// Pure static validation for customer registration fields.
/// All methods return true on success and set <paramref name="error"/> on failure.
/// </summary>
public static class CustomerValidator
{
    // Rules: letters and spaces only, no digits or punctuation.
    public static bool ValidateFullName(string fullName, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            error = "Full name is required.";
            return false;
        }

        foreach (char c in fullName)
        {
            if (!char.IsLetter(c) && c != ' ')
            {
                error = "Full name must contain letters only (no digits or symbols).";
                return false;
            }
        }

        return true;
    }

    // Rules: exactly 9 digits.
    public static bool ValidateNationalId(string nationalId, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(nationalId))
        {
            error = "Customer ID is required.";
            return false;
        }

        if (nationalId.Length != 9)
        {
            error = "Customer ID must be exactly 9 digits.";
            return false;
        }

        foreach (char c in nationalId)
        {
            if (!char.IsDigit(c))
            {
                error = "Customer ID must contain digits only.";
                return false;
            }
        }

        return true;
    }

    // Rules: valid Israeli phone — 10 digits starting with "05" (dashes and spaces allowed).
    public static bool ValidatePhone(string phone, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(phone))
        {
            error = "Phone number is required.";
            return false;
        }

        // Strip common separators before validating
        string cleaned = phone.Replace("-", "").Replace(" ", "");

        if (cleaned.Length != 10)
        {
            error = "Phone must be 10 digits (e.g. 050-1234567).";
            return false;
        }

        foreach (char c in cleaned)
        {
            if (!char.IsDigit(c))
            {
                error = "Phone must contain digits only (dashes allowed).";
                return false;
            }
        }

        if (!cleaned.StartsWith("05"))
        {
            error = "Israeli phone must start with 05 (e.g. 050-1234567).";
            return false;
        }

        return true;
    }

    // Rules: valid email format.
    public static bool ValidateEmail(string email, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            error = "Email is required.";
            return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address == email)
                return true;
        }
        catch { }

        error = "Please enter a valid email address (e.g. user@example.com).";
        return false;
    }
}
