using MauiBlazorWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace MauiBlazorWeb.Web.Services;

public static class AuthorizationPolicies
{
    public static void RegisterPolicies(AuthorizationOptions options)
    {
        options.AddPolicy("RequireAdminRole",
            policy => policy.RequireRole(ApplicationRoles.Admin));

        options.AddPolicy("RequireJudgeRole",
            policy => policy.RequireRole(ApplicationRoles.Judge, ApplicationRoles.Admin));

        options.AddPolicy("RequireModeratorRole",
            policy => policy.RequireRole(ApplicationRoles.Moderator, ApplicationRoles.Admin));

        options.AddPolicy("RequireUserRole",
            policy => policy.RequireRole(ApplicationRoles.User, ApplicationRoles.TrialUser, ApplicationRoles.Admin));

        options.AddPolicy("RequireShowHolderRole",
            policy => policy.RequireRole(ApplicationRoles.ShowHolder, ApplicationRoles.Admin));

        options.AddPolicy("RequireShowManagementRole",
            policy => policy.RequireRole(ApplicationRoles.ShowHolder, ApplicationRoles.Admin));
    }
}