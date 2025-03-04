namespace AuthService.Entities;

public class OrganizationUserRole
{
    public long OrganizationId { get; set; }
    public long UserId { get; set; } // ID пользователя
    public long OrganizationRoleId { get; set; } // ID роли внутри организации

    public Organization Organization { get; set; }
    public ApplicationUser User { get; set; } // Применяем `IdentityUser<long>`
    public OrganizationRole OrganizationRole { get; set; }
}