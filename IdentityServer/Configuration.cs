using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                //new IdentityResource.Email(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "Email"
                    }
                }
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo", new string[] { "rc.api.garndma" }),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = { "ApiOne" }
                },
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Code,  
                    RequirePkce = true,

                    RedirectUris = { "https://localhost:44388/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44388/Home/Index" },

                    AllowedScopes = {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //IdentityServerConstants.StandardScopes.Email,
                        "rc.scope",
                    },

                    // puts all the claims in the id token
                    AlwaysIncludeUserClaimsInIdToken = true,
                   // AllowOfflineAccess = true,
                    RequireConsent = false,
                },
                 new Client {
                    ClientId = "client_id_dotnet",
                    ClientSecrets = { new Secret("client_secret_dotnet".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Hybrid,
                   // RequirePkce = true,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    
                    PostLogoutRedirectUris = { "https://localhost:44300" },

                    AllowedScopes = { 
                       
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                         "rc.scope",
                         
                        //IdentityServerConstants.StandardScopes.Email,
                    },

                    // puts all the claims in the id token
                    AlwaysIncludeUserClaimsInIdToken = true,
                   // AllowOfflineAccess = true,
                    RequireConsent = false,
                },


            };
    }
}
