namespace ZenCare.WinUI.Helpers;

public static class AuthStorage
{
    public static string? Token { get; set; }
    public static string? Username { get; set; }
    public static List<string> Roles { get; set; } = new List<string>();
}
