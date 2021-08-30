using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Models
{
    public class Salida
    {
        public string NombreCliente { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }

    }
}
