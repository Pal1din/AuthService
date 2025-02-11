using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options): base(options)
    { }
}