using Microsoft.Owin;
using Owin;
using System.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Web.Script.Serialization;

[assembly: OwinStartupAttribute(typeof(SSO_DotNet_Consumer.Startup))]
namespace SSO_DotNet_Consumer
{
    public partial class Startup {
        private static readonly string _clientId = ConfigurationManager.AppSettings["clientId"];
        private static readonly string _redirectUri = ConfigurationManager.AppSettings["selfUrl"];
        private static readonly string _authority = ConfigurationManager.AppSettings["authorityUrl"];
        private static readonly string _clientSecret = ConfigurationManager.AppSettings["clientSecret"];
        
        public void Configuration(IAppBuilder app) {
           // ConfigureAuth(app);
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // Sets the ClientId, authority, RedirectUri as obtained from web.config
                    ClientId = _clientId,
                    Authority = _authority,
                    RedirectUri = _redirectUri,
                 //  CallbackPath = new PathString("/Contact.aspx"),
                    
                    // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                    PostLogoutRedirectUri = "https://localhost:44300",
                    Scope = "openid profile rc.scope",
                    
                    //Scope = "rc.scope",
                  
                    // ResponseType is set to request the id_token - which contains basic information about the signed-in user
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    // ValidateIssuer set to false to allow personal and work accounts from any organization to sign in to your application
                    // To only allow users from a single organizations, set ValidateIssuer to true and 'tenant' setting in web.config to the tenant name
                    // To allow users from only a list of specific organizations, set ValidateIssuer to true and use ValidIssuers parameter
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false // This is a simplification
                    },
                    // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailed,
                        AuthorizationCodeReceived = (context) =>
                        {
                            string authorizationCode = context.Code;
                            var accessToken = "";
                            var tokenType = "";

                            var tokenUrl = _authority + "connect/token";

                            using (WebClient client = new WebClient())
                            {
                                var values = new NameValueCollection();
                                values["grant_type"] = "authorization_code";
                                values["code"] = authorizationCode;
                                values["redirect_uri"] = _redirectUri;
                                values["client_id"] = _clientId;
                                values["client_secret"] = _clientSecret;

                                var response = client.UploadValues(tokenUrl, values);

                                var responseString = Encoding.Default.GetString(response);
                                var jsonSerializer = new JavaScriptSerializer();
                                var responseMessage = jsonSerializer.Deserialize<TokenResult>(responseString);
                                 accessToken = responseMessage.access_token;
                                 tokenType = responseMessage.token_type;
                             

                            }
                            var userUrl = _authority + "connect/userinfo";
                            var authorization = tokenType + " " + accessToken;

                            using (WebClient client = new WebClient())
                            {
                                var values = new NameValueCollection();
                                client.Headers["Content-Type"] = "application/json";
                                client.Headers.Add("Authorization", authorization);

                                var response = client.DownloadString(userUrl);

                                var jsonSerializer = new JavaScriptSerializer();
                              
                            }
                            return Task.FromResult(0);
                        }
                    }
                    
                }
            );
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }


  //      public void ConfigureAuth(IAppBuilder app)
  //{
  //          app.UseCookieAuthentication
  //  app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
  //          app.

  //  app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
  //  app.UseCookieAuthentication(new CookieAuthenticationOptions());

  //  app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
  //  {
  //    ClientId = _clientId,
  //    ClientSecret = _clientSecret,
  //    Authority = _authority,
  //    RedirectUri = _redirectUri,
  //    ResponseType = OpenIdConnectResponseType.CodeIdToken,
  //    Scope = OpenIdConnectScope.OpenIdProfile,
  //    TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" },
  //    Notifications = new OpenIdConnectAuthenticationNotifications
  //    {
  //      AuthorizationCodeReceived = async n =>
  //      {
  //        // Exchange code for access and ID tokens
  //        var tokenClient = new TokenClient($"{_authority}/v1/token", _clientId, _clientSecret);

  //        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(n.Code, _redirectUri);
  //        if (tokenResponse.IsError)
  //        {
  //          throw new Exception(tokenResponse.Error);
  //        }

  //        var userInfoClient = new UserInfoClient($"{_authority}/v1/userinfo");
  //        var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

  //        var claims = new List<Claim>(userInfoResponse.Claims)
  //        {
  //          new Claim("id_token", tokenResponse.IdentityToken),
  //          new Claim("access_token", tokenResponse.AccessToken)
  //        };

  //        n.AuthenticationTicket.Identity.AddClaims(claims);
  //      },
  //    },
  //  });
  //}
    }
}
