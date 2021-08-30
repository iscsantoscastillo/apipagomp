using ApiPagoMP.AD;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ApiPagoMP.Helpers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var token = authHeader.Parameter;
                                
                Conexion cnn = new Conexion();
                string cadenaBasicAuth = cnn.stCadenaBasicAuth(Constantes.BASIC_AUTH_KEY);
                if (token != null && token != String.Empty)
                {
                    if (token.Equals(cadenaBasicAuth))
                    {
                        
                        var claims = new Claim[] {                        
                            new Claim(ClaimTypes.Name, "ok"),
                        };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    }
                }                
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }            

            return AuthenticateResult.Fail("Error in Invalid Username or Password");
        }
    }
}
