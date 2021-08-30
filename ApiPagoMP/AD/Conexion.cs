using Microsoft.Extensions.Configuration;
using System.IO;

namespace ApiPagoMP.AD
{
    public class Conexion
    {
        public string cnCadena(string basedatos)
        {           
            var configuation = GetConfiguration();
            string con = configuation.GetSection("ConnectionStrings").GetSection(basedatos).Value.ToString();
            return con;
        }
        public string stCadenaBasicAuth(string seccion)
        {
            var configuation = GetConfiguration();
            string con = configuation.GetSection("AppSettings").GetSection(seccion).Value.ToString();
            return con;
        }


        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
