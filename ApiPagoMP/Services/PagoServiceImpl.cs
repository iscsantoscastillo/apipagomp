using ApiPagoMP.Models;
using ApiPagoMP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Services
{
    public class PagoServiceImpl : IPagoService
    {
        IPagoRepo _IPagoRepo;
        public PagoServiceImpl(IPagoRepo pagoRepo) {
            this._IPagoRepo = pagoRepo;
        }
        public Salida validarReferencia(Entrada entrada)
        {
            return this._IPagoRepo.validarReferencia(entrada);
        }

        public bool EsMontoValido(Entrada entrada) {
            return this._IPagoRepo.EsMontoValido(entrada);
        }

        public Comercio GetComercio(string comercioID) {
            return this._IPagoRepo.GetComercio(comercioID);
        }

        public Comercio GetComercio(string usuario, string contrasena)
        {
            return this._IPagoRepo.GetComercio(usuario, contrasena);
        }

        public void GrabarBitacora(string json) {
            this._IPagoRepo.GrabarBitacora(json);
        }

        public Error ConsultarError(string error)
        {
            return this._IPagoRepo.ConsultarError(error);
        }

        public int GrabarPago(Entrada entrada)
        {
            return this._IPagoRepo.GrabarPago(entrada);
        }

        public bool ExisteNotiPago(Entrada entrada)
        {
            return this._IPagoRepo.ExisteNotiPago(entrada);
        }
        public bool ExisteTransaccion(Entrada entrada) {
            return this._IPagoRepo.ExisteTransaccion(entrada);
        }
        public int GenerarAbono(Entrada entrada) {
            return this._IPagoRepo.GenerarAbono(entrada);
        }
        public Entrada ConsultarComercio(Entrada entrada) {
            return this._IPagoRepo.ConsultarComercio(entrada);
        }
        public int ActualizarPago(Entrada entrada) {
            return this._IPagoRepo.ActualizarPago(entrada);
        }
    }
}
