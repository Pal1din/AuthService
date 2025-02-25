using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService;

public class AuthDbContextMigrationFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        // Укажите строку подключения (можно получить из файла конфигурации или задать напрямую)
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=identity_server;User Id=postgres;Password=123;");

        return new AuthDbContext(optionsBuilder.Options);
    }
}