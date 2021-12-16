using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["sub"] = "userid";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://localhost:5001";
        options.RequireHttpsMetadata = false;

        options.Audience = "api1";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            //RoleClaimType = "role",
            //NameClaimType = "unique_name"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("myPolicy", policy => policy.RequireRole("admin").RequireClaim("client_group", "qingraingroup"));
});
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
