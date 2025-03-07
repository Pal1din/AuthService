namespace AuthService.Models;

public class RegisterModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
}