namespace InvenTrack.API.Common;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Staff = "Staff";
    public const string AdminOrManager = $"{Admin},{Manager}";
}
