namespace MarksBaseballCards.Shared.Common;

/// <summary>Role names used for authorization across the API and client.</summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string SystemAdmin = "SystemAdmin";

    public static readonly string[] All = { Admin, SystemAdmin };
}
