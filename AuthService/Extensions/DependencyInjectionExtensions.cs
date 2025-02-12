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

    
    internal static WebApplicationBuilder AddIdentityServer(this WebApplicationBuilder builder)
    {
        // Настроим ресурсы (какие поля будет содержать jwt токен при определенном scope)
        ApiResource[] resources =
        [
            //{name} в конструкторе отвечает за поле aud в jwt токене
            new ApiResource(IdentityServerConstants.StandardScopes.OpenId)
            {
                UserClaims =
                {
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Profile,
                },
                Scopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                }
            },
            new ApiResource("vehicle.resource")
            {
                UserClaims =
                    { },
                Scopes = new List<string>
                {
                    "vehicle.scope"
                },
            }
        ];

//Создаем все scope сервера
        ApiScope[] scopes =
        [
            new ApiScope("vehicle.scope", "vehicle scope"),
            new ApiScope(IdentityServerConstants.StandardScopes.OpenId, "Open ID Client"),
            new ApiScope(IdentityServerConstants.StandardScopes.Profile, "Profile Client"),
        ];

//Создаем клиентов которые будут обращаться к identityServer
        Client[] clients =
        [
            new Client
            {
                ClientId = "vehicle-service",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowAccessTokensViaBrowser = true,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AccessTokenType = AccessTokenType.Jwt,
                AllowedScopes =
                {
                    "vehicle.scope",
                    IdentityServerConstants.StandardScopes.OpenId,
                },
                RequireConsent = false
            }
        ];

        builder.Services.AddIdentityServer()
            .AddInMemoryClients(clients)
            .AddInMemoryApiResources(resources)
            .AddInMemoryApiScopes(scopes)
            .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://auth-service";
                options.Audience = "full.access";
                options.RequireHttpsMetadata = false;
            });
        return builder;
    }
}