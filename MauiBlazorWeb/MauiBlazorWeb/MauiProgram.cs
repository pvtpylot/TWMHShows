    using MauiBlazorWeb.Services;
using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using Microsoft.Extensions.DependencyInjection; // REQUIRED for AddHttpClient
using Microsoft.Maui.Hosting;

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
            builder.Services.AddScoped<IErrorHandler, DefaultErrorHandler>();

            // Register HttpClient for injection into HttpClientFactory
            builder.Services.AddSingleton<HttpClient>(sp => HttpClientHelper.GetHttpClient());

            // Register custom HttpClientFactory (required by MauiDataService)
            builder.Services.AddScoped<MauiBlazorWeb.Services.IHttpClientFactory, HttpClientFactory>();

            // Data services
            builder.Services.AddScoped<IDataService, MauiDataService>();
            builder.Services.AddScoped<IShowService, MauiShowService>();
            builder.Services.AddScoped<IShowClassService, MauiShowClassService>();
            builder.Services.AddScoped<IEntryService, MauiEntryService>();
            builder.Services.AddScoped<IResultService, MauiResultService>();

            builder.Services.AddScoped<AuthHeaderHandler>();

            builder.Services.AddHttpClient("ApiClient", client =>
            {
#if ANDROID
                client.BaseAddress = new Uri("https://10.0.2.2:7157");
#else
                client.BaseAddress = new Uri("https://localhost:7157");
#endif
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

            return builder.Build();
        }
    }
}