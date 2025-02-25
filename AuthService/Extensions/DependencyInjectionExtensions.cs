using AuthService.AccessServices;
using AuthService.Entities;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static WebApplicationBuilder AddAuthDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();
        return builder;
    }

    internal static WebApplicationBuilder MigrateDb(this WebApplicationBuilder builder)
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        dbContext.Database.Migrate();
        return builder;
    }
    
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddRazorPages();
        return builder;
    }

    
    internal static WebApplicationBuilder AddIdentityServer(this WebApplicationBuilder builder, IdentityServerSettings settings)
    {
        // Настроим ресурсы (какие поля будет содержать jwt токен при определенном scope)
        var resources = settings.Resources.Select(x => new ApiResource(x.Name, x.DisplayName)
        {
            Scopes = x.Scopes,
            UserClaims = x.Claims
        });

//Создаем все scope сервера
        var scopes = settings.Scopes.Select(x =>
        {
            if (x is { Name: not null, DisplayName: not null })
                return new ApiScope(x.Name, x.DisplayName);
            if (x.Name is not null && x.DisplayName is null)
                return new ApiScope(x.Name);
            return new ApiScope();
        });

//Создаем клиентов которые будут обращаться к identityServer
        var clients = settings.Clients.Select(x => new Client
        {
            ClientId = x.ClientId,
            AllowedGrantTypes = x.AllowedGrantTypes,
            ClientSecrets = { new Secret(x.ClientSecret.Sha256()) },
            AllowAccessTokensViaBrowser = x.AllowAccessTokensViaBrowser,
            AlwaysSendClientClaims = x.AlwaysSendClientClaims,
            AlwaysIncludeUserClaimsInIdToken = x.AlwaysIncludeUserClaimsInIdToken,
            AccessTokenType = x.AccessTokenType,
            RedirectUris = x.RedirectUris,
            PostLogoutRedirectUris = x.PostLogoutRedirectUris,
            AllowedScopes = x.AllowedScopes,
            RequireConsent = x.RequireConsent,
            AllowedCorsOrigins = x.AllowedCorsOrigins,
            RequirePkce = x.RequirePkce,
            RequireClientSecret = x.RequireClientSecret,
        });
        
        builder.Services.AddIdentityServer()
            .AddInMemoryClients(clients)
            .AddInMemoryApiResources(resources)
            .AddInMemoryApiScopes(scopes)
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://auth-service";
                options.Audience = "full.access";
                options.RequireHttpsMetadata = false;
            });
        builder.Services.AddControllersWithViews();
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy("AllowLocalhost5173", policy =>
        //     {
        //         policy.WithOrigins("")
        //             .AllowAnyHeader()
        //             .AllowAnyMethod();
        //     });
        // });
        return builder;
    }
}