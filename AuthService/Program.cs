using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Настроим IdentityServer как SSO
builder.Services.AddIdentityServer()
    .AddInMemoryClients(new List<Client>
    {
        new Client
        {
            ClientId = "vehicle-service",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "logs.read" }
        }
    })
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new ApiScope("logs.read", "Read logs")
    })
    .AddAspNetIdentity<ApplicationUser>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://auth-service";
        options.Audience = "logs.read";
        options.RequireHttpsMetadata = false;
    });

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
}

var app = builder.Build();

app.UseIdentityServer();
app.UseAuthorization();

app.MapPost("/register", async ([FromBody] RegisterModel model, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) =>
{
    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
    var result = await userManager.CreateAsync(user, model.Password);
    if (!result.Succeeded) return Results.BadRequest(result.Errors);
    
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new ApplicationRole("User"));
    }
    await userManager.AddToRoleAsync(user, "User");
    
    return Results.Ok("User created with role 'User'");
});

app.Run();

public class RegisterModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }  // Дополнительные поля
    public string LastName { get; set; }
}