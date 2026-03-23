using BetFounders.CentralBackend.Common.MappingProfiles.Users;
using BetFounders.CentralBackend.Common.Services;
using BetFounders.CentralBackend.Common.Services.Abstractions;
using BetFounders.CentralBackend.Data.Database;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using System.Security.Claims;

namespace BetFounders.CentralBackend.App.Infrastructure.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection BootstrapCentralBackendServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .ConfigureAutoMapper()
            .ConfigureBlazorAndUI()
            .ConfigureAuthentication()
            .ConfigureDatabaseAndRepositories(configuration)
            .ConfigureBusinessServices();
    }

    public static WebApplication BootstrapCentralBackendPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthEndpoints();
        app.MapRazorComponents<Components.App>().AddInteractiveServerRenderMode();

        return app;
    }

    public static async Task BootstrapDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<DbConnectionFactory>();
        await DbSeeder.SeedAsync(dbFactory);
    }

    private static IServiceCollection ConfigureBlazorAndUI(this IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddRadzenComponents();
        return services;
    }

    private static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
        return services;
    }

    private static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

        services.AddAuthorization();
        services.AddCascadingAuthenticationState();
        return services;
    }

    private static IServiceCollection ConfigureDatabaseAndRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddSingleton(new DbConnectionFactory(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        return services;
    }

    private static IServiceCollection ConfigureBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        return services;
    }

    private static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/auth/callback", async (HttpContext context, IMemoryCache cache, string ticket) =>
        {
            if (cache.TryGetValue(ticket, out User user))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role?.Name ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                cache.Remove(ticket);

                return Results.Redirect("/");
            }

            return Results.Redirect("/login?error=invalid_ticket");
        });

        app.MapGet("/api/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        });

        return app;
    }
}