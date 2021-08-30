using ApiPagoMP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Repository
{
    public interface IPagoRepo
    {
        public Salida validarReferencia(Entrada entrada);
    }
}
