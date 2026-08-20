namespace ZenCare.Model.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Employee = "Employee";
    public const string Client = "Client";

    public const string AdminOrEmployee = Admin + "," + Employee;
}
