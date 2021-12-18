using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using QingRain.IdentityServer.IdentityServer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
const string connectionString = @"Server=localhost;Port=3306;Database=IdentityServerDB;Uid=root;Pwd=123321;";

var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddIdentityServer()
.AddTestUsers(TestUsers.Users)
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = b => b.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion,
     sql => sql.MigrationsAssembly(migrationsAssembly));
})
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = b => b.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion, sql => sql.MigrationsAssembly(migrationsAssembly));
})
.AddDeveloperSigningCredential();

var app = builder.Build();
InitializeDatabase(app);
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();

static void InitializeDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    context.Database.Migrate();

    if (!context.Clients.Any())
    {
        foreach (var client in Config.Clients)
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
        foreach (var resource in Config.IdentityResources)
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiResources.Any())
    {
        foreach (var resource in Config.Apis)
        {
            context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
        foreach (var scope in Config.ApiScopes)
        {
            context.ApiScopes.Add(scope.ToEntity());
        }
        context.SaveChanges();
    }
}