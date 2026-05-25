namespace ClinicVets.g3.Validation;

/// <summary>
/// Pure static validation for visit fields.
/// </summary>
public static class VisitValidator
{
    public static bool ValidateReason(string reason, out string error)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            error = "Reason cannot be empty.";
            return false;
        }
        if (reason.Length > 500)
        {
            error = "Reason must not exceed 500 characters.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateDateTime(DateTime visitDateTime, out string error)
    {
        // Allow 1 minute of clock skew tolerance
        if (visitDateTime > DateTime.Now.AddMinutes(1))
        {
            error = "Visit date/time cannot be in the future.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateAnimalId(int animalId, out string error)
    {
        if (animalId <= 0)
        {
            error = "Animal must be selected.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateDiagnosis(string diagnosis, out string error)
    {
        if (diagnosis.Length > 1000)
        {
            error = "Diagnosis must not exceed 1000 characters.";
            return false;
        }
        error = string.Empty;
        return true;
    }
}
