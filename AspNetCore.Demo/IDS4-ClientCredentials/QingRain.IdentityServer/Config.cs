using IdentityServer4.Models;

namespace QingRain.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> Apis => new List<ApiResource>
        {
            new ApiResource("api1", "My First API")
            {
               Scopes={"api1"}
            }
        };
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
             new ApiScope("api1")
        };
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                 ClientId = "client",
                 // no interactive user, use the clientid/secret for authentication
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  // secret for authentication
                  ClientSecrets =
                   {
                       new Secret("secret".Sha256())
                   },
                                       // scopes that client has access to
                  AllowedScopes = { "api1"},
                   Claims=
                  {
                      new ClientClaim("role","qingrainrole"),
                      new ClientClaim("group", "qingraingroup")
                  }
            }
        };
    }
}
