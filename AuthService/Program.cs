using AuthService.AccessServices;
using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);
var identityServerSettings = builder.Configuration.GetSection("IdentityServer").Get<IdentityServerSettings>();
builder.AddAuthDbContext()
    .AddIdentityServer(identityServerSettings!)
    .MigrateDb()
    .ConfigureServices();


var app = builder.Build();

app.AddEndpoints();

app.Run();