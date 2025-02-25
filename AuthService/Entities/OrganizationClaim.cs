namespace AuthService.Entities;

public class OrganizationClaim
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }  // Организация
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }

    public Organization Organization { get; set; }
}