namespace ClinicVets.g1.Validation;

/// <summary>
/// Pure static validation for employee registration fields.
/// All methods return true on success and set <paramref name="error"/> to a
/// user-friendly message on failure.
/// </summary>
public static class EmployeeValidator
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

        foreach (char c in fullName.Trim())
        {
            if (!char.IsLetter(c) && c != ' ')
            {
                error = "Full name must contain letters only.";
                return false;
            }
        }

        return true;
    }

    // CFG target function 1 — used for testing diagrams.
    // Rules: 6-8 chars, at most 2 digits, all other chars must be English letters.
    public static bool ValidateUsername(string username, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            error = "Username is required.";
            return false;
        }

        if (username.Length < 6 || username.Length > 8)
        {
            error = "Username must be 6–8 characters long.";
            return false;
        }

        int digitCount = 0;
        foreach (char c in username)
        {
            if (char.IsDigit(c))
                digitCount++;
            else if (!char.IsAsciiLetter(c))
            {
                error = "Username may only contain English letters and digits.";
                return false;
            }
        }

        if (digitCount > 2)
        {
            error = "Username may contain at most 2 digits.";
            return false;
        }

        return true;
    }

    // CFG target function 2 — used for testing diagrams.
    // Rules: 8-10 chars, at least 1 letter, 1 digit, 1 special char from {!, #, $, ,}.
    public static bool ValidatePassword(string password, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(password))
        {
            error = "Password is required.";
            return false;
        }

        if (password.Length < 8 || password.Length > 10)
        {
            error = "Password must be 8–10 characters long.";
            return false;
        }

        bool hasLetter = false;
        bool hasDigit = false;
        bool hasSpecial = false;
        char[] allowedSpecials = new char[] { '!', '#', '$', ',' };

        foreach (char c in password)
        {
            if (char.IsLetter(c)) hasLetter = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (Array.IndexOf(allowedSpecials, c) >= 0) hasSpecial = true;
        }

        if (!hasLetter)
        {
            error = "Password must contain at least one letter.";
            return false;
        }

        if (!hasDigit)
        {
            error = "Password must contain at least one digit.";
            return false;
        }

        if (!hasSpecial)
        {
            error = "Password must contain at least one special character (!, #, $, ,).";
            return false;
        }

        return true;
    }

    // Rules: exactly 4 digits.
    public static bool ValidateEmployeeNumber(string employeeNumber, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            error = "Employee number is required.";
            return false;
        }

        if (employeeNumber.Length != 4)
        {
            error = "Employee number must be exactly 4 digits.";
            return false;
        }

        foreach (char c in employeeNumber)
        {
            if (!char.IsDigit(c))
            {
                error = "Employee number must contain digits only.";
                return false;
            }
        }

        return true;
    }

    // Rules: valid email format (must contain '@' and a domain part).
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

    // Rules: exactly 9 digits (Israeli national ID format).
    public static bool ValidateNationalId(string nationalId, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(nationalId))
        {
            error = "National ID is required.";
            return false;
        }

        if (nationalId.Length != 9)
        {
            error = "National ID must be exactly 9 digits.";
            return false;
        }

        foreach (char c in nationalId)
        {
            if (!char.IsDigit(c))
            {
                error = "National ID must contain digits only.";
                return false;
            }
        }

        return true;
    }
}
