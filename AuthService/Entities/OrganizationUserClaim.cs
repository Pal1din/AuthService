using Microsoft.AspNetCore.Identity;

namespace AuthService.Entities;

public class OrganizationUserClaim
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public long UserId { get; set; }
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }

    public Organization Organization { get; set; }
    public ApplicationUser User { get; set; }
}