namespace ClinicVets.g2.Validation;

/// <summary>
/// Pure static validation for animal registration fields.
/// </summary>
public static class AnimalValidator
{
    // Rules: letters only.
    public static bool ValidateName(string name, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Animal name is required.";
            return false;
        }

        if (!name.Trim().All(char.IsLetter))
        {
            error = "Animal name must contain letters only.";
            return false;
        }

        return true;
    }

    // Rules: positive decimal between 0.1 and 100 (kg).
    public static bool ValidateWeight(decimal weight, out string error)
    {
        error = string.Empty;

        if (weight < 0.1m || weight > 100m)
        {
            error = "Weight must be between 0.1 and 100 kg.";
            return false;
        }

        return true;
    }

    // Rules: not in the future, not before 01/01/2000.
    public static bool ValidateBirthDate(DateTime birthDate, out string error)
    {
        error = string.Empty;
        var date = birthDate.Date;

        if (date > DateTime.Today)
        {
            error = "Birth date cannot be in the future.";
            return false;
        }

        if (date < new DateTime(2000, 1, 1))
        {
            error = "Birth date cannot be before 01/01/2000.";
            return false;
        }

        return true;
    }

    // Rules: ownerId must be a positive integer that exists in the Customers table.
    // The repository lookup is done in AnimalService; this helper validates the int value only.
    public static bool ValidateOwnerId(int ownerId, out string error)
    {
        error = string.Empty;

        if (ownerId <= 0)
        {
            error = "Owner is required.";
            return false;
        }

        return true;
    }
}
