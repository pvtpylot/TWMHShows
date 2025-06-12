using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Components;
using MauiBlazorWeb.Web.Components.Account;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;
using MauiBlazorWeb.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiBlazorWeb.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Add your user service registration here
builder.Services.AddScoped<IUserService, WebUserService>();

// Add Auth services used by the Web app
builder.Services.AddAuthentication(options =>
{
    // Ensure that unauthenticated clients redirect to the login page rather than receive a 401 by default.
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IUserModelObjectRepository, UserModelObjectRepository>();
builder.Services.AddScoped<IDataService, WebDataService>();

// Register repositories
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IShowClassRepository, ShowClassRepository>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();

// Register services
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IShowClassService, ShowClassService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IResultService, ResultService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Needed for external clients to log in
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Apply migrations & create database if needed at startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MauiBlazorWeb.Shared._Imports).Assembly);

// Needed for external clients to log in
app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();
// Needed for Identity Blazor components
app.MapAdditionalIdentityEndpoints();

//Add the weather API endpoint and require authorization
app.MapGet("/api/weather", async (IWeatherService weatherService) =>
{
    var forecasts = await weatherService.GetWeatherForecastsAsync();
    return Results.Ok(forecasts);
}).RequireAuthorization();

app.MapGet("/api/userModelObjects", async (IDataService dataService, string applicationUserId) =>
{
    if (string.IsNullOrEmpty(applicationUserId))
    {
        return Results.BadRequest("The 'applicationUserId' parameter is required.");
    }
    // Your existing logic here
    return Results.Ok(await dataService.GetAllUserModelObjectsAsync(applicationUserId));
}).RequireAuthorization();


app.MapGet("/api/userModelObjects/{id}", async (IDataService dataService, string id) =>
{
    var result = await dataService.GetUserModelObjectByIdAsync(id);
    return result != null ? Results.Ok(result) : Results.NotFound();
}).RequireAuthorization();

// Show API endpoints
app.MapGet("/api/shows", async (IShowService showService) =>
{
    return Results.Ok(await showService.GetAllShowsAsync());
}).RequireAuthorization();

app.MapGet("/api/shows/{id}", async (string id, IShowService showService) =>
{
    var show = await showService.GetShowByIdAsync(id);
    return show != null ? Results.Ok(show) : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/api/shows/judge/{judgeId}", async (string judgeId, IShowService showService) =>
{
    return Results.Ok(await showService.GetShowsByJudgeIdAsync(judgeId));
}).RequireAuthorization();

app.MapPost("/api/shows", async (ShowDto showDto, IShowService showService) =>
{
    var result = await showService.CreateShowAsync(showDto);
    return Results.Created($"/api/shows/{result.Id}", result);
}).RequireAuthorization();

app.MapPut("/api/shows/{id}", async (string id, ShowDto showDto, IShowService showService) =>
{
    if (id != showDto.Id)
        return Results.BadRequest("ID mismatch");
    
    var result = await showService.UpdateShowAsync(showDto);
    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/api/shows/{id}", async (string id, IShowService showService) =>
{
    var result = await showService.DeleteShowAsync(id);
    return result ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Near the app.Run() call, add:
if (app.Environment.IsDevelopment())
{
    // Allow connections from any hostname in development
    app.Urls.Add("https://127.0.0.1:7157");
    app.Urls.Add("https://*:7157");
}

app.Run();
