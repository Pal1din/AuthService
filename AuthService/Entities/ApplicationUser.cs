using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Entities;

public sealed class ApplicationUser : IdentityUser<long>
{
    [MaxLength(16)]
    public string? FirstName { get; set; }
    [MaxLength(16)]
    public string? LastName { get; set; }
    
    [ForeignKey("Organization")]
    public long? OrganizationId { get; set; } = null;
    public Organization Organization { get; set; }
}