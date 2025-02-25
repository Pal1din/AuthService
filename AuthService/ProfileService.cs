using System.Security.Claims;
using AuthService.Entities;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthService;

public class ProfileService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager): IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        if (user == null) return;

        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(JwtClaimTypes.PreferredUserName, user.UserName!)
        };

        // Добавляем роли в токен
        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        // Загружаем permissions из ролей
        foreach (var role in roles)
        {
            var identityRole = await roleManager.FindByNameAsync(role);
            if (identityRole != null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(identityRole);
                claims.AddRange(roleClaims);
            }
        }

        context.IssuedClaims.AddRange(claims);
        
        if(user.OrganizationId.HasValue)
        {
            context.IssuedClaims.Add(new Claim("organization_id", user.OrganizationId.ToString()));
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}