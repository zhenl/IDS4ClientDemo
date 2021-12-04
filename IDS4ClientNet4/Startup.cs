using IDS4ClientNet4;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Startup))]
namespace IDS4ClientNet4
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "http://localhost:4010",
                ClientId = "net4mvcclient",
                ClientSecret = "secret3",
                RedirectUri = "http://localhost:49816/signin-oidc",//Net4MvcClient's URL

                PostLogoutRedirectUri = "http://localhost:49816",
                ResponseType = "id_token token",
                RequireHttpsMetadata = false,

                Scope = "openid profile myapi",

                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                SignInAsAuthenticationType = "Cookies",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        return Task.FromResult(0);
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var id_token_claim = n.OwinContext.Authentication.User.Claims.FirstOrDefault(x => x.Type == "id_token");
                            if (id_token_claim != null)
                            {
                                n.ProtocolMessage.IdTokenHint = id_token_claim.Value;
                            }
                        }
                        return Task.FromResult(0);
                    }
                }

            });

        }
    }
}