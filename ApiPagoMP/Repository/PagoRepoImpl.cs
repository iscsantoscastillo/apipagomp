using ApiPagoMP.AD;
using ApiPagoMP.Helpers;
using ApiPagoMP.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPagoMP.Repository
{
    public class PagoRepoImpl : IPagoRepo
    {
        Logger log = LogManager.GetCurrentClassLogger();
        public Salida validarReferencia(Entrada entrada)
        {
            Salida salida = null;
            log.Info("Ingresando al método validarReferencia");
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_consultar";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        
                        sqlCommand.Parameters.Add("@CLAVE_REFERENCIA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CLAVE_REFERENCIA"].Value = entrada.Referencia;
                        
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente para la validación de la referencia");
                                salida = new Salida();
                                while (sqlDataReader.Read())
                                {
                                    salida.NombreCliente = sqlDataReader["cliente"].ToString();
                                    salida.MontoMinimo = Decimal.Parse(sqlDataReader["monto_min"].ToString());
                                    salida.MontoMaximo = Decimal.Parse(sqlDataReader["monto_max"].ToString());
                                }
                            }
                        }
                    }
                    log.Info("Ok al buscar la refencia");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Ocurrió un error al momento de consultar la información en el servidor: " + ex.Message);
                throw new Exception(ex.Message);
            }

            return salida;
        }
    }
}
