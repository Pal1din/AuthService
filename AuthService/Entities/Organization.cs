namespace AuthService.Entities;

public class Organization
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}