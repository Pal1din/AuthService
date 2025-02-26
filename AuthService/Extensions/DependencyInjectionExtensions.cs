using System.Security.Cryptography;
using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static WebApplicationBuilder AddAuthDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();
        return builder;
    }

    internal static WebApplicationBuilder MigrateDb(this WebApplicationBuilder builder)
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        dbContext.Database.Migrate();
        return builder;
    }
    
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddRazorPages();
        return builder;
    }

    
    internal static WebApplicationBuilder AddIdentityServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection()
            .PersistKeysToDbContext<AuthDbContext>()
            .SetApplicationName("IdentityServer")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        builder.Services.AddIdentityServer(options =>
            {
                
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = db => 
                    db.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
                    });
            })
            .AddOperationalStore(op =>
            {
                op.ConfigureDbContext = db => db.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName));
                // Автоматическая очистка устаревших токенов (каждый час)
                op.EnableTokenCleanup = true;
                op.TokenCleanupInterval = 3600;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>()
            .AddKeyManagement()
            .AddSigningCredential(new SigningCredentials(
                new RsaSecurityKey(RSA.Create(2048)), SecurityAlgorithms.RsaSha256));

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://auth-service";
                options.Audience = "full.access";
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            });
        
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
        });
        
        builder.Services.AddControllersWithViews();
        return builder;
    }
}