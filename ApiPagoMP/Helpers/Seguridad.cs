using ApiPagoMP.Models;
using ApiPagoMP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiPagoMP.Helpers
{
    public class Seguridad
    {
        private  IPagoService _iPagoService;
        public Seguridad() { }
        public Seguridad(IPagoService pagoService)
        {
            _iPagoService = pagoService;
        }

        public Comercio GetComercio(string comercioID) {
            Comercio comercio = null;
            comercio = _iPagoService.GetComercio(comercioID);
            return comercio;
        }

        public Comercio GetComercio(string usuario, string contrasena)
        {
            Comercio comercio = null;
            comercio = _iPagoService.GetComercio(usuario, contrasena);
            return comercio;
        }

        /// <summary>
        /// Método para desencriptar la cadena enviada como Basic Auth
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public Usuario GetUsuario(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                Usuario usuario = new Usuario();
                usuario.Nombre = name;
                usuario.Contrasena = password;

                return usuario;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
