using AuthService.Entities;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Extensions;

internal static class WebApplicationEndpoints
{
    internal static WebApplication AddEndpoints(this WebApplication app)
    {
        DefaultEndpoints(app);
        // AddRegisterEndpoint(app);
        return app;
    }

    private static void DefaultEndpoints(this WebApplication app)
    {
        // app.UseCors("AllowLocalhost5173");
        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        // app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        app.MapDefaultControllerRoute();
        app.MapRazorPages()
            .RequireAuthorization();
        // app.MapControllerRoute(name: "default",
        //     pattern: "{controller=Home}/{action=Index}/{id?}");

    }

    private static void AddRegisterEndpoint(WebApplication app)
    {
        app.MapPost("/register", async ([FromBody] RegisterModel model, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) =>
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return Results.BadRequest(result.Errors);
    
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new ApplicationRole("User"));
            }
            await userManager.AddToRoleAsync(user, "User");
    
            return Results.Ok("User created with role 'User'");
        });
    }
}