using ApiPagoMP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Services
{
    public interface IPagoService
    {
        public Salida validarReferencia(Entrada entrada);
        public bool EsMontoValido(Entrada entrada);
        public Comercio GetComercio(string comercioID);
        public Comercio GetComercio(string usuario, string contrasena);
        public void GrabarBitacora(string json);
        public Error ConsultarError(string error);
        public bool GrabarPago(Entrada entrada);
        public bool ExisteNotiPago(Entrada entrada);
        public bool ExisteTransaccion(Entrada entrada);
    }
}
