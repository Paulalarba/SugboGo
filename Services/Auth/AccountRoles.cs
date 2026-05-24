namespace SugboGo.Services.Auth;

public static class AccountRoles
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string Partner = "Partner";
    public const string AdminOrClient = Admin + "," + Client;
    public const string AdminOrPartner = Admin + "," + Partner;

    public static string Normalize(string? role)
    {
        if (string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase)) return Admin;
        if (string.Equals(role, Partner, StringComparison.OrdinalIgnoreCase)) return Partner;
        return Client;
    }
}
