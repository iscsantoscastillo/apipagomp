using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Models
{
    public class Comercio
    {
        public int ID { get; set; }
        public string ComercioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public bool Estatus { get; set; }
        public string CajaMP { get; set; }
        public string ReferenciaMP { get; set; }
        public string CveFormaPagoMP { get; set; }
        public string PlataformaMP { get; set; }
        public string SucursalMP { get; set; }
    }
}
