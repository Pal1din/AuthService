using Microsoft.AspNetCore.Identity;

namespace AuthService.Entities;

public class ApplicationRole(string name) : IdentityRole<long>(name);