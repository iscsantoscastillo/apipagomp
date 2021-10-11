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
                    string sp = "sp_mpf_referencia_unica_validate";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        
                        sqlCommand.Parameters.Add("@referencia", SqlDbType.VarChar);
                        sqlCommand.Parameters["@referencia"].Value = entrada.Referencia;

                        sqlCommand.Parameters.Add("@plataforma", SqlDbType.VarChar);
                        sqlCommand.Parameters["@plataforma"].Value = entrada.PlataformaMP;

                        sqlCommand.Parameters.Add("@monto_referencia", SqlDbType.Decimal);
                        sqlCommand.Parameters["@monto_referencia"].Value = 0;
                        
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente para la validación de la referencia");
                                
                                while (sqlDataReader.Read())
                                {
                                    bool payable = Int32.Parse(sqlDataReader["payable"].ToString()) == 1 ? true : false;

                                    if (payable) {
                                        salida = new Salida();

                                        salida.NombreCliente = sqlDataReader["cliente"].ToString();
                                        salida.MontoMinimo = Decimal.Parse(sqlDataReader["min_amount"].ToString());
                                        salida.MontoMaximo = Decimal.Parse(sqlDataReader["max_amount"].ToString());
                                    }
                                    
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
        public bool EsMontoValido(Entrada entrada) {
            bool esValido = false;
            log.Info("Ingresando al método EsMontoValido");
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_mpf_referencia_unica_validate";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@referencia", SqlDbType.VarChar);
                        sqlCommand.Parameters["@referencia"].Value = entrada.Referencia;

                        sqlCommand.Parameters.Add("@plataforma", SqlDbType.VarChar);
                        sqlCommand.Parameters["@plataforma"].Value = entrada.PlataformaMP;

                        sqlCommand.Parameters.Add("@monto_referencia", SqlDbType.Decimal);
                        sqlCommand.Parameters["@monto_referencia"].Value = entrada.MontoPago;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente para la validación de la referencia");

                                while (sqlDataReader.Read())
                                {
                                    esValido = Int32.Parse(sqlDataReader["payable"].ToString()) == 1 ? true : false;                                   
                                }
                            }
                        }
                    }
                    log.Info("Ok al buscar EsMontoValido");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Ocurrió un error al momento de consultar la información en el servidor: " + ex.Message);
                throw new Exception(ex.Message);
            }

            return esValido;
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
                                    comercio.CajaMP = sqlDataReader["caja_mp"].ToString();
                                    comercio.ReferenciaMP = sqlDataReader["referencia_mp"].ToString();
                                    comercio.CveFormaPagoMP = sqlDataReader["cve_forma_pago_mp"].ToString();
                                    comercio.PlataformaMP = sqlDataReader["plataforma_mp"].ToString();
                                    comercio.SucursalMP = sqlDataReader["sucursal_mp"].ToString();
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

        public int GrabarPago(Entrada entrada) {
            int registro = 0;
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
                        sqlCommand.Parameters["@REFERENCIA"].Value = entrada.Referencia.ToUpper();

                        sqlCommand.Parameters.Add("@COMERCIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIO"].Value = entrada.Comercio;

                        sqlCommand.Parameters.Add("@MONTOPAGO", SqlDbType.Decimal);
                        sqlCommand.Parameters["@MONTOPAGO"].Value = entrada.MontoPago;

                        sqlCommand.Parameters.Add("@TRANSACCION", SqlDbType.VarChar);
                        sqlCommand.Parameters["@TRANSACCION"].Value = entrada.NumeroTransaccion;

                        sqlCommand.Parameters.Add("@FECHAHORATRX", SqlDbType.BigInt);                                                
                        sqlCommand.Parameters["@FECHAHORATRX"].Value = entrada.FechaHoraTransaccion;
                        
                        sqlCommand.Parameters.Add("@CLAVESUCURSAL", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CLAVESUCURSAL"].Value = entrada.ClaveSucursal;

                        sqlCommand.Parameters.Add("@USUARIOALTA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@USUARIOALTA"].Value = Constantes.SISTEMA;

                        SqlParameter outParm = new SqlParameter("@ID_DEVUELTO", SqlDbType.Int);
                        outParm.Direction = ParameterDirection.Output;
                        sqlCommand.Parameters.Add(outParm);


                        int afectacion = sqlCommand.ExecuteNonQuery();

                        registro = Int32.Parse(outParm.Value.ToString());

                        return registro;

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
        /// <summary>
        /// Verifica si: transaccion ya existe, comercio ya existe, clave_sucursal ya existe y estatus=1. Entonces devuelve true
        /// </summary>
        /// <param name="entrada"></param>
        /// <returns></returns>
        public bool ExisteTransaccion(Entrada entrada) {
            bool existe = false;

            log.Info("Ingresando al método ExisteTransaccion");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_notipago_con_trx";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@TRANSACCION", SqlDbType.VarChar);
                        sqlCommand.Parameters["@TRANSACCION"].Value = entrada.NumeroTransaccion;

                        sqlCommand.Parameters.Add("@COMERCIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIO"].Value = entrada.Comercio;
                        
                        sqlCommand.Parameters.Add("@CLAVE_SUCURSAL", SqlDbType.VarChar);
                        sqlCommand.Parameters["@CLAVE_SUCURSAL"].Value = entrada.ClaveSucursal;
                        
                        sqlCommand.Parameters.Add("@REFERENCIA", SqlDbType.VarChar);
                        sqlCommand.Parameters["@REFERENCIA"].Value = entrada.Referencia;


                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            existe = sqlDataReader.HasRows;

                            if (existe)
                            {
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
        public int GenerarAbono(Entrada entrada) {
            int registro = 0;

            log.Info("Ingresando al método GenerarAbono");

            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_sfc_generar_abono";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;                        

                        sqlCommand.Parameters.Add("@clave_solicitud", SqlDbType.VarChar);
                        sqlCommand.Parameters["@clave_solicitud"].Value = entrada.Referencia;

                        sqlCommand.Parameters.Add("@cve_sucursal", SqlDbType.VarChar);
                        sqlCommand.Parameters["@cve_sucursal"].Value = entrada.SucursalMP;

                        sqlCommand.Parameters.Add("@cve_vendedor", SqlDbType.VarChar);
                        sqlCommand.Parameters["@cve_vendedor"].Value = entrada.IDDevuelto.ToString();
                        
                        sqlCommand.Parameters.Add("@caja", SqlDbType.VarChar);
                        sqlCommand.Parameters["@caja"].Value = entrada.CajaMP;
                        
                        sqlCommand.Parameters.Add("@referencia", SqlDbType.VarChar);
                        sqlCommand.Parameters["@referencia"].Value = entrada.ReferenciaMP;

                        sqlCommand.Parameters.Add("@total_pagado", SqlDbType.VarChar);
                        sqlCommand.Parameters["@total_pagado"].Value = entrada.MontoPago.ToString();

                        sqlCommand.Parameters.Add("@cve_forma_pago", SqlDbType.VarChar);
                        sqlCommand.Parameters["@cve_forma_pago"].Value = entrada.CveFormaPagoMP;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente.");
                                while (sqlDataReader.Read())
                                {
                                    registro = Int32.Parse(sqlDataReader["id_registro"].ToString());
                                }                                
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
            return registro;
        }
        public Entrada ConsultarComercio(Entrada entrada) {            
            log.Info("Ingresando al método ConsultarComercio");
            
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_comercio_con";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@COMERCIO", SqlDbType.VarChar);
                        sqlCommand.Parameters["@COMERCIO"].Value = entrada.Comercio;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                log.Info("Se encontró la información correctamente.");                                                                

                                while (sqlDataReader.Read())
                                {
                                    entrada.CajaMP = sqlDataReader["caja_mp"].ToString();
                                    entrada.ReferenciaMP = sqlDataReader["referencia_mp"].ToString();
                                    entrada.CveFormaPagoMP = sqlDataReader["cve_forma_pago_mp"].ToString();
                                    entrada.PlataformaMP = sqlDataReader["plataforma_mp"].ToString();
                                    entrada.SucursalMP = sqlDataReader["sucursal_mp"].ToString();                                    

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
            return entrada;
        }
        public int ActualizarPago(Entrada entrada) {
            int filasAfectadas = 0;
            Conexion conexion = new Conexion();
            try
            {
                using (SqlConnection cnn = new SqlConnection(conexion.cnCadena(Constantes.BD_SOFT)))
                {
                    cnn.Open();
                    string sp = "sp_pmp_pago_notipago_update";
                    using (SqlCommand sqlCommand = new SqlCommand(sp, cnn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@ID_PAGO", SqlDbType.Int);
                        sqlCommand.Parameters["@ID_PAGO"].Value = entrada.IDDevuelto;

                        sqlCommand.Parameters.Add("@AS_ID_ABONO", SqlDbType.BigInt);
                        sqlCommand.Parameters["@AS_ID_ABONO"].Value = entrada.AsIDAbono;

                        sqlCommand.Parameters.Add("@USUARIO_ENV_SC", SqlDbType.VarChar);
                        sqlCommand.Parameters["@USUARIO_ENV_SC"].Value = Constantes.SISTEMA;
                       
                        SqlParameter outParm = new SqlParameter("@FILAS_AFECTADAS", SqlDbType.Int);
                        outParm.Direction = ParameterDirection.Output;
                        sqlCommand.Parameters.Add(outParm);

                        sqlCommand.ExecuteNonQuery();

                        filasAfectadas = Int32.Parse(outParm.Value.ToString());

                        return filasAfectadas;

                    }
                    log.Info("Se grabó correctamente en Notificacion de Pagos");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Error inesperado al actualiozar en ActualizarPago de Pagos " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
