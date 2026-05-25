namespace ClinicVets.g3.Validation;

/// <summary>
/// Pure static validation for visit fields.
/// Only veterinarians may open visits — enforce role in the form/service, not here.
/// </summary>
public static class VisitValidator
{
    // Rules: reason must be non-empty.
    public static bool ValidateReason(string reason, out string error)
        => throw new NotImplementedException();

    // Rules: visitDateTime must not be in the future beyond 1 minute (clock skew tolerance).
    public static bool ValidateDateTime(DateTime visitDateTime, out string error)
        => throw new NotImplementedException();

    // Rules: animalId must be positive and exist in the Animals table.
    // Existence check is done in VisitService; this validates the int value only.
    public static bool ValidateAnimalId(int animalId, out string error)
        => throw new NotImplementedException();
}
