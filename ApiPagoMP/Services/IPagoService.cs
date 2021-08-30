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
    }
}
