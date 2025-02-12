using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAuthDbContext()
    .AddIdentityServer()
    .MigrateDb();


var app = builder.Build();

app.UseIdentityServer()
    .UseAuthorization();

app.AddEndpoints();

app.Run();