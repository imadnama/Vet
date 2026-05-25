namespace ClinicVets.g3.Validation;

/// <summary>
/// Pure static validation for medicine inventory fields.
/// </summary>
public static class MedicineValidator
{
    // Rules: name must be non-empty.
    public static bool ValidateName(string name, out string error)
        => throw new NotImplementedException();

    // Rules: price must be > 0.
    public static bool ValidatePrice(decimal price, out string error)
        => throw new NotImplementedException();

    // Rules: quantity must be >= 0.
    public static bool ValidateQuantity(int quantity, out string error)
        => throw new NotImplementedException();
}
