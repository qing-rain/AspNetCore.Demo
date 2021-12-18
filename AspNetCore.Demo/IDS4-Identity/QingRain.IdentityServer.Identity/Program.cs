using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QingRain.IdentityServer.Identity.Data.AspNetIdentity;
using QingRain.IdentityServer.IdentityServer;
using Serilog;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
const string connectionString = @"Server=localhost;Port=3306;Database=IdentityServerDB;Uid=root;Pwd=123321;";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<ApplicationUser>()
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
InitializeClientDatabase(app);

InitializeUserDatabase(app);
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();

void InitializeClientDatabase(IApplicationBuilder app)
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

void InitializeUserDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;

    context.Database.Migrate();

    var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var alice = userMgr.FindByNameAsync("alice").Result;
    if (alice == null)
    {
        alice = new ApplicationUser
        {
            UserName = "alice"
        };
        var result = userMgr.CreateAsync(alice, "Pass123$").Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        Log.Debug("alice created");
    }
    else
    {
        Log.Debug("alice already exists");
    }

    var bob = userMgr.FindByNameAsync("bob").Result;
    if (bob == null)
    {
        bob = new ApplicationUser
        {
            UserName = "bob"
        };
        var result = userMgr.CreateAsync(bob, "Pass123$").Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        Log.Debug("bob created");
    }
    else
    {
        Log.Debug("bob already exists");
    }
}