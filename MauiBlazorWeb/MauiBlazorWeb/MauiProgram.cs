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
                options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(ApplicationRoles.Admin));
                options.AddPolicy("RequireJudgeRole", policy => policy.RequireRole(ApplicationRoles.Judge, ApplicationRoles.Admin));
                options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole(ApplicationRoles.Moderator, ApplicationRoles.Admin));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole(ApplicationRoles.User, ApplicationRoles.TrialUser, ApplicationRoles.Admin));

                foreach (var role in ApplicationRoles.AllRoles)
                {
                    options.AddPolicy($"RequiresRole_{role}", policy => policy.RequireRole(role));
                }
            });
            builder.Services.AddScoped<INetworkDiagnostics, DefaultNetworkDiagnostics>();
            builder.Services.AddScoped<IAuthenticationService, DefaultAuthenticationService>();
            builder.Services.AddScoped<ISecureStorageProvider, MauiSecureStorageProvider>();
            builder.Services.AddScoped<ITokenStorage, TokenStorage>();
            builder.Services.AddScoped<MauiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
                sp.GetRequiredService<MauiAuthenticationStateProvider>());

            // Device/shared services
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddScoped<IUserService, MauiUserService>();

            // Auth helpers
            builder.Services.AddSingleton<IAuthService, MauiAuthService>();
            builder.Services.AddScoped<IRoleService, MauiRoleService>();

            // Missing registrations for MauiDataService dependencies
            builder.Services.AddScoped<IHttpClientFactory, HttpClientFactory>();
            builder.Services.AddScoped<IErrorHandler, DefaultErrorHandler>();

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

            // Data services
            builder.Services.AddScoped<IDataService, MauiDataService>();
            builder.Services.AddScoped<IShowService, MauiShowService>();
            builder.Services.AddScoped<IShowClassService, MauiShowClassService>();
            builder.Services.AddScoped<IEntryService, MauiEntryService>();
            builder.Services.AddScoped<IResultService, MauiResultService>();

            return builder.Build();
        }
    }
}