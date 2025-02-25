using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService;

public class OrganizationManager(AuthDbContext context)
{
    private UserManager<ApplicationUser> _userManager;

    public async Task<IdentityResult> CreateAsync(Organization organization)
    {
        context.Organizations.Add(organization);
        await context.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Organization organization)
    {
        context.Organizations.Remove(organization);
        await context.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Organization organization)
    {
        context.Organizations.Update(organization);
        await context.SaveChangesAsync();
        return IdentityResult.Success;
    }
    
    public async Task<IEnumerable<ApplicationUser>> GetOrganizationUsers(Organization organization)
    {
        return await context.Users.Where(x => x.OrganizationId == organization.Id)
            .ToListAsync();
    }

    public async Task<IdentityResult> AddExistingUserAsync(Organization organization, ApplicationUser user)
    {
        //todo проверка наличия организации, проверка наличия юзера
        user.OrganizationId = organization.Id;
        await _userManager.UpdateAsync(user);
        return IdentityResult.Success;
    }
    
    public async Task<IdentityResult> DeleteFromOrganization(Organization organization, ApplicationUser user)
    {
        //todo проверка наличия организации, проверка наличия юзера
        user.OrganizationId = null;
        await _userManager.UpdateAsync(user);
        return IdentityResult.Success;
    }
}