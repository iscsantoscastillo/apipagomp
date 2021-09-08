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
    }
}
