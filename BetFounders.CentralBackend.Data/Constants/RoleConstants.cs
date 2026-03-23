namespace BetFounders.CentralBackend.Data.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const long AdminId = 1;
    public const string AdminDescription = "Full access";

    public const string Manager = "Manager";
    public const long ManagerId = 2;
    public const string ManagerDescription = "Can manage users";

    public const string Viewer = "Viewer";
    public const long ViewerId = 3;
    public const string ViewerDescription = "Read-only access"; 
}