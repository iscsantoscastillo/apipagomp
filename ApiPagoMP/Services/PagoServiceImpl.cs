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
    }
}
