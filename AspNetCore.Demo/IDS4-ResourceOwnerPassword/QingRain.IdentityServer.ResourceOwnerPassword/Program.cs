using QingRain.IdentityServer.ResourceOwnerPassword;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServer()
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(Config.Users)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddDeveloperSigningCredential();

var app = builder.Build();
app.UseIdentityServer();


app.Run();
