using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Entities;

public class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, long>(options), IDataProtectionKeyContext
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationRole> OrganizationRoles { get; set; }
    public DbSet<OrganizationUserRole> OrganizationUserRoles { get; set; }
    public DbSet<OrganizationClaim> OrganizationClaims { get; set; }
    public DbSet<OrganizationUserClaim> OrganizationUserClaims { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Организация ↔ Глобальная роль
        builder.Entity<OrganizationRole>()
            .HasOne(or => or.Organization)
            .WithMany(o => o.OrganizationRoles)
            .HasForeignKey(or => or.OrganizationId);

        builder.Entity<OrganizationRole>()
            .HasOne(or => or.Role)
            .WithMany()
            .HasForeignKey(or => or.RoleId);

        // Пользователь ↔ Организация ↔ Роль
        builder.Entity<OrganizationUserRole>()
            .HasKey(our => new { our.OrganizationId, our.UserId });

        builder.Entity<OrganizationUserRole>()
            .HasOne(our => our.Organization)
            .WithMany()
            .HasForeignKey(our => our.OrganizationId);

        builder.Entity<OrganizationUserRole>()
            .HasOne(our => our.User)
            .WithMany()
            .HasForeignKey(our => our.UserId);

        builder.Entity<OrganizationUserRole>()
            .HasOne(our => our.OrganizationRole)
            .WithMany()
            .HasForeignKey(our => our.OrganizationRoleId);

        // Организация ↔ Claims
        builder.Entity<OrganizationClaim>()
            .HasOne(oc => oc.Organization)
            .WithMany()
            .HasForeignKey(oc => oc.OrganizationId);

        // Пользователь ↔ Организация ↔ Claims
        builder.Entity<OrganizationUserClaim>()
            .HasOne(ouc => ouc.Organization)
            .WithMany()
            .HasForeignKey(ouc => ouc.OrganizationId);

        builder.Entity<OrganizationUserClaim>()
            .HasOne(ouc => ouc.User)
            .WithMany()
            .HasForeignKey(ouc => ouc.UserId);
        
        builder.Entity<DataProtectionKey>().ToTable("DataProtectionKeys");
    }
}