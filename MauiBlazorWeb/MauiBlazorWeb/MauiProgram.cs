using MauiBlazorWeb.Services;
using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace MauiBlazorWeb
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

#if ANDROID
            builder.Services.AddTransient<AndroidHttpMessageHandler>();
#endif

            // Register authentication services
            builder.Services.AddAuthorizationCore(options => 
            {
                // Basic policies
                options.AddPolicy("RequireAuthenticatedUser", policy => 
                    policy.RequireAuthenticatedUser());
                
                // Role-based policies
                options.AddPolicy("RequireAdminRole", policy => 
                    policy.RequireRole(ApplicationRoles.Admin));
                
                options.AddPolicy("RequireJudgeRole", policy => 
                    policy.RequireRole(ApplicationRoles.Judge, ApplicationRoles.Admin));
                
                options.AddPolicy("RequireModeratorRole", policy => 
                    policy.RequireRole(ApplicationRoles.Moderator, ApplicationRoles.Admin));
                
                options.AddPolicy("RequireUserRole", policy => 
                    policy.RequireRole(ApplicationRoles.User, ApplicationRoles.TrialUser, ApplicationRoles.Admin));

                // You can also add dynamic policies based on the roles
                foreach (var role in ApplicationRoles.AllRoles)
                {
                    options.AddPolicy($"RequiresRole_{role}", policy => 
                        policy.RequireRole(role));
                }
            });
            builder.Services.AddScoped<INetworkDiagnostics, DefaultNetworkDiagnostics>();
            builder.Services.AddScoped<IAuthenticationService, DefaultAuthenticationService>();
            builder.Services.AddScoped<ISecureStorageProvider, MauiSecureStorageProvider>();
            builder.Services.AddScoped<ITokenStorage, TokenStorage>();
            builder.Services.AddScoped<MauiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
                sp.GetRequiredService<MauiAuthenticationStateProvider>());

            // Add device-specific services used by the MauiBlazorWeb.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();

            // Add the user service registration here
            builder.Services.AddScoped<IUserService, MauiUserService>();

            // Add these service registrations
            builder.Services.AddSingleton<IAuthService, MauiAuthService>();
            builder.Services.AddScoped<IRoleService, MauiRoleService>();

#if ANDROID
            builder.Services.AddSingleton(sp => 
            {
                var handler = sp.GetRequiredService<AndroidHttpMessageHandler>();
                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(HttpClientHelper.BaseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };
                return httpClient;
            });
#else
            builder.Services.AddSingleton(_ => new HttpClient
            {
                BaseAddress = new Uri(HttpClientHelper.BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            });
#endif

            // Register data services
            builder.Services.AddScoped<IDataService, MauiDataService>();
            builder.Services.AddScoped<IShowService, MauiShowService>();
            builder.Services.AddScoped<IShowClassService, MauiShowClassService>();
            builder.Services.AddScoped<IEntryService, MauiEntryService>();
            builder.Services.AddScoped<IResultService, MauiResultService>();

            return builder.Build();
        }
    }
}