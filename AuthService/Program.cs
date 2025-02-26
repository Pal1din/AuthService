using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAuthDbContext()
    .AddIdentityServer()
    .MigrateDb()
    .ConfigureServices();


var app = builder.Build();

app.AddEndpoints();

app.Run();