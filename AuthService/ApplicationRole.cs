using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole<long>
{
    public ApplicationRole(string name): base(name)
    {
        
    }
}