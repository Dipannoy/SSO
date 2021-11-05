using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Configuration;
using System.Web.Http;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace SSO_DotNet_Consumer
{
    public partial class _Default : Page
    {

        private static readonly string _clientId = ConfigurationManager.AppSettings["clientId"];
        private static readonly string _redirectUri = ConfigurationManager.AppSettings["selfUrl"];
        private static readonly string _authority = ConfigurationManager.AppSettings["authorityUrl"];
        private static readonly string _clientSecret = ConfigurationManager.AppSettings["clientSecret"];
        protected void Page_Load(object sender, EventArgs e)
        {
            //var result = HttpContext.Current.Request.Form["code"] != null &&
            //                    HttpContext.Current.Request.Form["id_token"] != null;
            var result = false;
                                //&&
                                //HttpContext.Current.Request.Form["state"] != null;
            var accessToken = "";
            var tokenType = "";
            var idToken = "";
            int expireTime = 0;
        
  
            if (!result)
            {

            }
            else
            {
                var code = HttpContext.Current.Request.Form["code"];
                try
                {
                    var tokenUrl = _authority + "connect/token";

                    using (WebClient client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["grant_type"] = "authorization_code";
                        values["code"] = code;
                        values["redirect_uri"] = _redirectUri;
                        values["client_id"] = _clientId;
                        values["client_secret"] = _clientSecret;

                        var response = client.UploadValues(tokenUrl, values);

                        var responseString = Encoding.Default.GetString(response);
                        var jsonSerializer = new JavaScriptSerializer();
                        var responseMessage = jsonSerializer.Deserialize<TokenResult>(responseString);
                        accessToken = responseMessage.access_token;
                        tokenType = responseMessage.token_type;
                        idToken = responseMessage.id_token;
                        expireTime = int.Parse(responseMessage.expires_in); 

                        //if (responseMessage != null)
                        //{
                        //    SessionSGD.SaveObjToSession<string>(responseMessage.token_type, SessionName.Common_TokenType);
                        //    SessionSGD.SaveObjToSession<string>(responseMessage.access_token, SessionName.Common_AccessToken);
                        //    SessionSGD.SaveObjToSession<string>(DateTime.Now.AddSeconds(int.Parse(responseMessage.expires_in)).ToString(), SessionName.Common_ExpireIn);

                        //}
                        //else
                        //{
                        //    PageBase.ErrorRedirect("Null Return in SSO Get Token, code: " + code);
                        //}
                        //return responseMessage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                   // PageBase.ErrorRedirect("Error in SSO Get Token: " + ex.Message);
                }

                try
                {
                    var userUrl = _authority + "connect/userinfo";
                    var authorization = tokenType + " " + accessToken;

                    using (WebClient client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        client.Headers["Content-Type"] = "application/json";
                        client.Headers.Add("Authorization", authorization);

                        var response = client.DownloadString(userUrl);

                        var jsonSerializer = new JavaScriptSerializer();
                       // var responseMessage = jsonSerializer.Deserialize<User>(response);
                        //if (responseMessage != null)
                        //{
                        //    SessionSGD.SaveObjToSession<User>(responseMessage, SessionName.Common_SSOUser);
                        //}
                        //else
                        //{
                        //    PageBase.ErrorRedirect("Null Return in SSO Get UserInfo, authorization: " + authorization);
                        //}
                        //return responseMessage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                   // PageBase.ErrorRedirect("Error in SSO Get UserInfo : " + ex.Message + " > tokenType:" + tokentype + " > accessToken:" + accesstoken);
                }
            }
        }
    }

    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string id_token { get; set; }
    }
}