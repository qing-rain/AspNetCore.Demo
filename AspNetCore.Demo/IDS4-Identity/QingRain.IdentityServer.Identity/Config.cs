// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace QingRain.IdentityServer.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> IdentityResources =>
           new List<IdentityResource>
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("custom_profile",new []{ JwtClaimTypes.Role,JwtRegisteredClaimNames.UniqueName})
           };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API",new []{ JwtClaimTypes.Role})
                {
                    Scopes= { "api1.read","api1.write" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1.read",new []{ JwtRegisteredClaimNames.UniqueName}),
                new ApiScope("api1.write")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    
                    //ClientClaimsPrefix=string.Empty,
                    Claims=
                    {
                        new ClientClaim(JwtClaimTypes.Role, "client-role"),
                        new ClientClaim(JwtRegisteredClaimNames.UniqueName, "console-client")
                    },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "api1.read" }

                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    //ClientClaimsPrefix=string.Empty,
                    Claims=
                    {
                        new ClientClaim(JwtClaimTypes.Role, "client-role"),
                        new ClientClaim(JwtRegisteredClaimNames.UniqueName, "mvc-client")
                    },
                    AlwaysSendClientClaims=true,
                    AlwaysIncludeUserClaimsInIdToken=true,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                         "api1.write",
                         "custom_profile"
                    },

                    AllowOfflineAccess=true,
                },
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",

                    //ClientClaimsPrefix=string.Empty,
                    Claims=
                    {
                        new ClientClaim(JwtClaimTypes.Role, "client-role"),
                        new ClientClaim(JwtRegisteredClaimNames.UniqueName, "js-client")
                    },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "https://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "https://localhost:5003" },

                    AlwaysSendClientClaims=true,
                    AlwaysIncludeUserClaimsInIdToken=true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1.read",
                        "custom_profile"
                    }
                }
            };
    }
}