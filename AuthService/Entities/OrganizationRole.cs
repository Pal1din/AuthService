namespace AuthService.Entities;

public class OrganizationRole
{
    public long Id { get; set; }  // Уникальный ID
    public long OrganizationId { get; set; }  // Организация
    public long RoleId { get; set; }  // ID глобальной роли (AspNetRoles)

    public Organization Organization { get; set; }
    public ApplicationRole Role { get; set; }  // Связываем с `IdentityRole<long>`
}