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

        public Comercio GetComercio(string comercioID) {
            Comercio comercio = null;
            
            log.Info("Ingresando al método GetComercio");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_consultar_ref";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@COMERCIOID", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIOID"].Value = comercioID;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente.");
                                comercio = new Comercio();
                                while (sqlDataReader.Read())
                                {
                                    comercio.ID = Int32.Parse(sqlDataReader["id"].ToString());
                                    comercio.ComercioID = sqlDataReader["comercio"].ToString();
                                    comercio.NombreUsuario = sqlDataReader["usuario"].ToString();
                                    comercio.Contrasena = sqlDataReader["contrasena"].ToString();
                                    comercio.Estatus = Int32.Parse(sqlDataReader["estatus"].ToString()) == 1? true:false;

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
            return comercio;
        }

        public Comercio GetComercio(string usuario, string contrasena)
        {
            Comercio comercio = null;

            log.Info("Ingresando al método GetComercio(por usuario y contrasena)");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_consultar_usuario_contrasena";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@USUARIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@USUARIO"].Value = usuario;
                        
                        sqlCommand.Parameters.Add("@CONTRASENA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CONTRASENA"].Value = contrasena;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente.");
                                comercio = new Comercio();
                                while (sqlDataReader.Read())
                                {
                                    comercio.ID = Int32.Parse(sqlDataReader["id"].ToString());
                                    comercio.ComercioID = sqlDataReader["comercio"].ToString();
                                    comercio.NombreUsuario = sqlDataReader["usuario"].ToString();
                                    comercio.Contrasena = sqlDataReader["contrasena"].ToString();
                                    comercio.Estatus = Int32.Parse(sqlDataReader["estatus"].ToString()) == 1 ? true : false;

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
            return comercio;
        }

        public void GrabarBitacora(string json) {
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_bitacora_ins";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@JSON", SqlDbType.VarChar);
                        sqlCommand.Parameters["@JSON"].Value = json;

                        sqlCommand.Parameters.Add("@USUARIOALTA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@USUARIOALTA"].Value = Constantes.SISTEMA;

                        sqlCommand.ExecuteNonQuery();
                       
                    }
                    log.Info("Se grabó correctamente en la bitácora");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Error inesperado al grabar en la bitácora " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public Error ConsultarError(string error)
        {
            Error err = null;

            log.Info("Ingresando al método ConsultarError");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_error_con";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@CODIGO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CODIGO"].Value = error;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente.");
                                err = new Error();
                                while (sqlDataReader.Read())
                                {
                                    err.Codigo = sqlDataReader["codigo"].ToString();
                                    err.Descripcion = sqlDataReader["descripcion"].ToString();
                                    
                                }
                            }
                        }
                    }
                    log.Info("Ok al buscar el error");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Ocurrió un error al momento de consultar la información en el servidor: " + ex.Message);
                throw new Exception(ex.Message);
            }
            return err;
        }

        public bool GrabarPago(Entrada entrada) {
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_notipago_ins";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@REFERENCIA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@REFERENCIA"].Value = entrada.Referencia;

                        sqlCommand.Parameters.Add("@COMERCIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIO"].Value = entrada.Comercio;

                        sqlCommand.Parameters.Add("@MONTOPAGO", SqlDbType.Decimal);
                        sqlCommand.Parameters["@MONTOPAGO"].Value = entrada.MontoPago;

                        sqlCommand.Parameters.Add("@TRANSACCION", SqlDbType.VarChar);
                        sqlCommand.Parameters["@TRANSACCION"].Value = entrada.NumeroTransaccion;

                        sqlCommand.Parameters.Add("@FECHAHORATRX", SqlDbType.DateTime);
                        DateTime fechaHoraTrx = new DateTime(entrada.FechaHoraTransaccion);
                        sqlCommand.Parameters["@FECHAHORATRX"].Value = fechaHoraTrx;
                        
                        sqlCommand.Parameters.Add("@CLAVESUCURSAL", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CLAVESUCURSAL"].Value = entrada.ClaveSucursal;

                        sqlCommand.Parameters.Add("@USUARIOALTA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@USUARIOALTA"].Value = Constantes.SISTEMA;


                        int afectacion = sqlCommand.ExecuteNonQuery();

                        return afectacion > 0;

                    }
                    log.Info("Se grabó correctamente en Notificacion de Pagos");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Error inesperado al grabar en Notificacion de Pagos " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool ExisteNotiPago(Entrada entrada) {
            bool existe = false;

            log.Info("Ingresando al método ExisteNotiPago");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_notipago_con";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@REFERENCIA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@REFERENCIA"].Value = entrada.Referencia;
                        
                        sqlCommand.Parameters.Add("@COMERCIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIO"].Value = entrada.Comercio;


                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            existe = sqlDataReader.HasRows;

                            if (existe) {
                                log.Info("Se encontró la información correctamente.");
                            } 
                                                                                        
                        }
                    }
                    log.Info("Ok al buscar la NotiPago");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Ocurrió un error al momento de consultar la información en el servidor: " + ex.Message);
                throw new Exception(ex.Message);
            }
            return existe;
        }
    }
}
