﻿using ApiPagoMP.Models;
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

        public bool GrabarPago(Entrada entrada)
        {
            return this._IPagoRepo.GrabarPago(entrada);
        }

        public bool ExisteNotiPago(Entrada entrada)
        {
            return this._IPagoRepo.ExisteNotiPago(entrada);
        }
    }
}
