using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Entities;

public class ApplicationUser : IdentityUser<long>
{
    [MaxLength(16)]
    public string? FirstName { get; set; }
    [MaxLength(16)]
    public string? LastName { get; set; }
}