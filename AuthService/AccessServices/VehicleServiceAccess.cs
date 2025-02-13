using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace AuthService.AccessServices;

internal static class VehicleServiceAccess
{
    internal static ApiResource Resource = new ApiResource("vehicle.resource")
    {
        UserClaims = { "role", "permission" },
        Scopes = new List<string>
        {
            "read",
            "write"
        },
    };

    internal static readonly Client Client = new Client
    {
        ClientId = "vehicle-service",
        AllowedGrantTypes = GrantTypes.Code,
        ClientSecrets = { new Secret("secret".Sha256()) },
        AllowAccessTokensViaBrowser = true,
        AlwaysSendClientClaims = true,
        AlwaysIncludeUserClaimsInIdToken = true,
        AccessTokenType = AccessTokenType.Jwt,
        RedirectUris = { "https://localhost:7165/swagger/oauth2-redirect.html" },
        PostLogoutRedirectUris = { "https://localhost:7165/swagger/" },
        AllowedScopes =
        {
            "read",
            "write",
            IdentityServerConstants.StandardScopes.OpenId,
        },
        RequireConsent = false,
        AllowedCorsOrigins = ["https://localhost:7165"],
    };
}