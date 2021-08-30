using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Models
{
    public class Entrada
    {
        public string Referencia { get; set; }
        public string Comercio { get; set; }
        public decimal MontoPago { get; set; }
        public string NumeroTransaccion { get; set; }
        public long FechaHoraTransaccion { get; set; }
        public string ClaveSucursal { get; set; }
        public string Evento { get; set; }
    }
}
