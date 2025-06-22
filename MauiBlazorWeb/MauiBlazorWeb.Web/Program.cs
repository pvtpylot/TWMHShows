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
using System.Security.Claims;
using MauiBlazorWeb.Web.Services.Mappers;
using MauiBlazorWeb.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiBlazorWeb.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Add your user service registration here
builder.Services.AddScoped<IUserService, WebUserService>();

// Add this near the top with other service registrations
builder.Services.AddScoped<IRoleService, WebRoleService>();

// Required for role management
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Add Auth services used by the Web app
builder.Services.AddAuthentication(options =>
{
    // Ensure that unauthenticated clients redirect to the login page rather than receive a 401 by default.
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});

// Add this near the top of your Program.cs after other service registrations
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMauiApps", policy =>
    {
        policy
            .AllowAnyOrigin()  
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization(options => 
{
    AuthorizationPolicies.RegisterPolicies(options);
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

// Register mappers
builder.Services.AddScoped<IEntityMapper<UserModelObject, UserModelObjectDto>, UserModelObjectMapper>();

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore-swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// This makes the server accessible on all network interfaces
builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(5000); // HTTP port
    options.ListenAnyIP(7157, configure => configure.UseHttps()); // HTTPS port
});

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

app.UseCors("AllowMauiApps");

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

app.MapPost("/identity/mobilelogin", async (HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    try
    {
        // Read the request body as form data
        var form = await context.Request.ReadFormAsync();
        
        if (!form.TryGetValue("email", out var email) || !form.TryGetValue("password", out var password))
        {
            return Results.BadRequest("Email and password are required");
        }
        
        // Attempt to sign in
        var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            // Find the user
            var user = await userManager.FindByEmailAsync(email);
            
            // Get user roles
            var roles = await userManager.GetRolesAsync(user);
            
            // Generate tokens
            var refreshToken = Guid.NewGuid().ToString();
            var accessToken = await userManager.GenerateUserTokenAsync(user, "Default", "APIAccessToken");
            
            // Store refresh token in user claims or userstore
            var refreshClaim = new Claim("RefreshToken", refreshToken);
            await userManager.AddClaimAsync(user, refreshClaim);
            
            // Create response with roles
            var response = new
            {
                tokenType = "Bearer",
                accessToken = accessToken,
                expiresIn = 3600, // 1 hour
                refreshToken = refreshToken,
                userId = user.Id,
                roles = roles.ToArray()
            };
            
            return Results.Ok(response);
        }
        else if (result.IsLockedOut)
        {
            return Results.StatusCode(423); // Locked
        }
        else if (result.RequiresTwoFactor)
        {
            return Results.StatusCode(403); // Need 2FA
        }
        else
        {
            return Results.Unauthorized();
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"Login error: {ex.Message}");
    }
}).DisableAntiforgery();

// Add these API endpoints after the existing userModelObjects endpoints
app.MapPost("/api/userModelObjects", async (IDataService dataService, UserModelObjectDto userModelObjectDto) =>
{
    var result = await dataService.CreateUserModelObjectAsync(userModelObjectDto);
    return Results.Created($"/api/userModelObjects/{result.Id}", result);
}).RequireAuthorization();

app.MapPut("/api/userModelObjects/{id}", async (string id, UserModelObjectDto userModelObjectDto, IDataService dataService) =>
{
    if (id != userModelObjectDto.Id)
        return Results.BadRequest("ID mismatch");
    
    var result = await dataService.UpdateUserModelObjectAsync(id, userModelObjectDto);
    return result.Id != null ? Results.Ok(result) : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/userModelObjects/{id}", async (string id, IDataService dataService) =>
{
    var result = await dataService.DeleteUserModelObjectAsync(id);
    return result ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Initialize application roles during startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    // Ensure all application roles exist
    foreach (var role in ApplicationRoles.AllRoles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
    
    // Optional: Create an admin user if none exists
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            First = "Admin",
            Last = "User"
        };
        
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
    }
}

// Add these API endpoints for role management
app.MapGet("/api/users/{userId}/roles", async (string userId, IRoleService roleService) =>
{
    var roles = await roleService.GetUserRolesAsync(userId);
    return Results.Ok(roles);
}).RequireAuthorization();

app.MapPost("/api/users/{userId}/roles", async (string userId, string role, IRoleService roleService) =>
{
    var result = await roleService.AddUserToRoleAsync(userId, role);
    return result ? Results.Ok() : Results.BadRequest("Failed to add role");
}).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Admin));

app.MapDelete("/api/users/{userId}/roles/{role}", async (string userId, string role, IRoleService roleService) =>
{
    var result = await roleService.RemoveUserFromRoleAsync(userId, role);
    return result ? Results.Ok() : Results.BadRequest("Failed to remove role");
}).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Admin));

app.MapGet("/api/roles", async (IRoleService roleService) =>
{
    var roles = await roleService.GetAllRolesAsync();
    return Results.Ok(roles);
}).RequireAuthorization();

// Add these API endpoints for user management
app.MapGet("/api/users", async (UserManager<ApplicationUser> userManager) =>
{
    var users = await userManager.Users
        .Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.First,
            LastName = u.Last,
            EmailConfirmed = u.EmailConfirmed,
            IsLockedOut = u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now
        })
        .ToListAsync();
    
    return Results.Ok(users);
}).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Admin));

app.MapGet("/api/users/{userId}", async (string userId, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.FindByIdAsync(userId);
    if (user == null)
        return Results.NotFound();
    
    var roles = await userManager.GetRolesAsync(user);
    
    var userDto = new UserDto
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        FirstName = user.First,
        LastName = user.Last,
        EmailConfirmed = user.EmailConfirmed,
        IsLockedOut = user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now,
        Roles = roles.ToList()
    };
    
    return Results.Ok(userDto);
}).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Admin));

app.Run();
