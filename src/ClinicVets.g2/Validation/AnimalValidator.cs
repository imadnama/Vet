namespace ClinicVets.g2.Validation;

/// <summary>
/// Pure static validation for animal registration fields.
/// </summary>
public static class AnimalValidator
{
    // Rules: letters only.
    public static bool ValidateName(string name, out string error)
        => throw new NotImplementedException();

    // Rules: positive decimal between 0.1 and 100 (kg).
    public static bool ValidateWeight(decimal weight, out string error)
        => throw new NotImplementedException();

    // Rules: not in the future, not before 01/01/2000.
    public static bool ValidateBirthDate(DateTime birthDate, out string error)
        => throw new NotImplementedException();

    // Rules: ownerId must be a positive integer that exists in the Customers table.
    // The repository lookup is done in AnimalService; this helper validates the int value only.
    public static bool ValidateOwnerId(int ownerId, out string error)
        => throw new NotImplementedException();
}
