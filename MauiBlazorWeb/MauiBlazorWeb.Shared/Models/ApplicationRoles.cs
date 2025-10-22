namespace MauiBlazorWeb.Shared.Models;

public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Judge = "Judge";
    public const string TrialUser = "TrialUser";
    public const string User = "User";
    public const string Moderator = "Moderator";
    public const string ShowHolder = "ShowHolder";

    public static readonly IReadOnlyList<string> AllRoles = new List<string>
    {
        Admin,
        Judge,
        TrialUser,
        User,
        Moderator,
        ShowHolder
    };
}