using ClinicVets.Core.Enums;
using ClinicVets.Core.Models;

namespace ClinicVets.Core.Session;

/// <summary>
/// Global singleton that holds the authenticated employee for the lifetime of
/// the application session. Set by g1's LoginForm on successful login.
/// Read by all forms to enforce role-based access.
/// </summary>
public static class CurrentUserSession
{
    public static Employee? CurrentUser { get; private set; }

    public static bool IsLoggedIn => CurrentUser is not null;

    public static bool IsVeterinarian =>
        CurrentUser?.Role == Role.Veterinarian;

    public static bool IsSecretary =>
        CurrentUser?.Role == Role.Secretary;

    public static void SetUser(Employee employee) =>
        CurrentUser = employee;

    public static void Clear() =>
        CurrentUser = null;
}
