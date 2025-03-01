using Duende.IdentityServer.Models;

namespace ClientImporter;

public class IdentityServerSettings
{
    public List<ClientSettings> Clients { get; set; } = new();
    public List<ScopeSettings> Scopes { get; set; } = new();
    public List<ApiResourceSettings> Resources { get; set; } = new();
}

public class ClientSettings
{
    public string ClientId { get; set; }
    public List<string> AllowedGrantTypes { get; set; }
    public string ClientSecret { get; set; }
    public bool AllowAccessTokensViaBrowser { get; set; }
    public bool AlwaysSendClientClaims { get; set; }
    public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
    public AccessTokenType AccessTokenType { get; set; }
    public List<string> RedirectUris { get; set; }
    public List<string> PostLogoutRedirectUris { get; set; }
    public List<string> AllowedScopes { get; set; }
    public List<string> AllowedCorsOrigins { get; set; }
    
    public bool RequirePkce { get; set; }
    public bool RequireClientSecret { get; set; }
    public bool RequireConsent { get; set; }
}

public class ScopeSettings
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
}

public class ApiResourceSettings
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Scopes { get; set; } = [];
    public List<string> Claims { get; set; } = [];
}