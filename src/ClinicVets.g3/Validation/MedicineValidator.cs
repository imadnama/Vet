namespace ClinicVets.g3.Validation;

/// <summary>
/// Pure static validation for medicine inventory fields.
/// </summary>
public static class MedicineValidator
{
    public static bool ValidateName(string name, out string error)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Medicine name cannot be empty.";
            return false;
        }
        if (name.Length > 200)
        {
            error = "Medicine name must not exceed 200 characters.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidatePrice(decimal price, out string error)
    {
        if (price <= 0)
        {
            error = "Price must be greater than 0.";
            return false;
        }
        if (price > 100000)
        {
            error = "Price seems unreasonably high.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateQuantity(int quantity, out string error)
    {
        if (quantity < 0)
        {
            error = "Quantity cannot be negative.";
            return false;
        }
        if (quantity > 10000)
        {
            error = "Quantity seems unreasonably high.";
            return false;
        }
        error = string.Empty;
        return true;
    }
}
