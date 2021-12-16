using QingRain.IdentityServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServer()
    .AddInMemoryApiResources(Config.Apis)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddDeveloperSigningCredential();

var app = builder.Build();
app.UseIdentityServer();
app.Run();
