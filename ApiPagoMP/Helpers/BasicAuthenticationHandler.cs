using ApiPagoMP.AD;
using ApiPagoMP.Models;
using ApiPagoMP.Repository;
using ApiPagoMP.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
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
                var body = Request.Body;
                var todaCadena = authHeader.ToString();
                var soloCadena = todaCadena.Substring(Constantes.BASIC_AUTH.Length, todaCadena.Length - Constantes.BASIC_AUTH.Length);

                //Se desencripta cadena del Header y se obtienen usuario y contraseña 
                Seguridad seg = new Seguridad();
                Usuario usuario = seg.GetUsuario(soloCadena);

                //Seguridad seg = new Seguridad();
                PagoRepoImpl pagoService = new PagoRepoImpl();

                Comercio comercio = pagoService.GetComercio(usuario.Nombre, usuario.Contrasena);

                if (comercio is null) { 
                    return AuthenticateResult.Fail("No se encontró el usuario o la contraseña");
                }               

                var token = authHeader.Parameter;
                                
                //Conexion cnn = new Conexion();
                //string cadenaBasicAuth = cnn.stCadenaBasicAuth(Constantes.BASIC_AUTH_KEY);
                if (comercio.NombreUsuario.Equals(usuario.Nombre) && comercio.Contrasena.Equals(usuario.Contrasena))
                {
                    //if (token.Equals(cadenaBasicAuth))
                    //{
                        
                        var claims = new Claim[] {                        
                            new Claim(ClaimTypes.Name, "ok"),
                        };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    //}
                }                
            }
            catch(Exception ex)
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }            

            return AuthenticateResult.Fail("Error in Invalid Username or Password");
        }
        
    }
}
