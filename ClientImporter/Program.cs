using AuthService.Entities;
using ClientImporter;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var cs = "Server=postgresql;Port=8000;Database=identity_server;User Id=gen_user;Password=%0m6E_H0qB3@nP;";
builder.Services.AddIdentityServer(options =>
    {

    })
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = db =>
            db.UseNpgsql(cs, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
            });
    })
    .AddOperationalStore(op =>
    {
        op.ConfigureDbContext = db => db.UseNpgsql(cs,
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName));
        // Автоматическая очистка устаревших токенов (каждый час)
        op.EnableTokenCleanup = true;
        op.TokenCleanupInterval = 3600;
    });
var settings = builder.Configuration.GetSection("IdentityServer").Get<IdentityServerSettings>();

var resources = settings.Resources.Select(x => Duende.IdentityServer.EntityFramework.Mappers.ApiResourceMappers.ToEntity(new ApiResource(x.Name, x.DisplayName)
{
    Scopes = x.Scopes,
    UserClaims = x.Claims
}));

//Создаем все scope сервера
var scopes = settings.Scopes.Select(x =>
{
    if (x is { Name: not null, DisplayName: not null })
        return Duende.IdentityServer.EntityFramework.Mappers.ScopeMappers.ToEntity(new ApiScope(x.Name, x.DisplayName));
    if (x.Name is not null && x.DisplayName is null)
        return Duende.IdentityServer.EntityFramework.Mappers.ScopeMappers.ToEntity(new ApiScope(x.Name));
    return Duende.IdentityServer.EntityFramework.Mappers.ScopeMappers.ToEntity(new ApiScope());
});

//Создаем клиентов которые будут обращаться к identityServer
var clients = settings.Clients.Select(x => Duende.IdentityServer.EntityFramework.Mappers.ClientMappers.ToEntity(new Client
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
}));
var host = builder.Build();
using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
context.ApiScopes.AddRange(scopes);
context.ApiResources.AddRange(resources);
context.Clients.AddRange(clients);
context.SaveChanges();