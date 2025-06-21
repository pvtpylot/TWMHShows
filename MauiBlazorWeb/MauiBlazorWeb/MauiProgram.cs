    using MauiBlazorWeb.Services;
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
            builder.Services.AddAuthorizationCore();
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