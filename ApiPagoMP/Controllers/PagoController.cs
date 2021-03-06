using ApiPagoMP.Helpers;
using ApiPagoMP.Models;
using ApiPagoMP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiPagoMP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PagoController : ControllerBase
    {
        Logger log = LogManager.GetCurrentClassLogger();
        private IPagoService _iPagoService;
        public PagoController(IPagoService pagoService) {
            this._iPagoService = pagoService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            return new string[] { "ApiPagoMP V. " + version };

        }

        [HttpPost("PagoMP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]//Basic Auth
        public async Task<IActionResult> AsignarContrasenia([FromBody] JsonElement ent)
        {
            Error error = null;                      
            string mensaje = null;
            Salida salida = null;
            try
            {
                var json = ent.GetRawText();
                //Grabar en bitácora el Json enviado
                this._iPagoService.GrabarBitacora(json);
                Entrada entrada = JsonConvert.DeserializeObject<Entrada>(json);                

                if (entrada != null)
                {
                    #region VALIDAR REFERENCIA
                    if (entrada.Evento.Equals(Constantes.VALIDAR_REFERENCIA))
                    {
                        //Validar el comercio
                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
                            entrada.CajaMP = comercio.CajaMP;
                            entrada.SucursalMP = comercio.SucursalMP;
                            entrada.PlataformaMP = comercio.PlataformaMP;
                            entrada.ReferenciaMP = comercio.ReferenciaMP;
                            entrada.CveFormaPagoMP = comercio.CveFormaPagoMP;

                            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                            var body = Request.Body;
                            var todaCadena = authHeader.ToString();
                            var soloCadena = todaCadena.Substring(Constantes.BASIC_AUTH.Length, todaCadena.Length - Constantes.BASIC_AUTH.Length);

                            //Se desencripta cadena del Header y se obtienen usuario y contraseña 
                            Seguridad seg = new Seguridad();
                            Usuario usuario = seg.GetUsuario(soloCadena);

                            if (!comercio.NombreUsuario.Equals(usuario.Nombre) ||
                                !comercio.Contrasena.Equals(usuario.Contrasena))
                            {

                                error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);

                                return Ok(new
                                {
                                    exitoso = false,
                                    codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                    mensajeError = error.Descripcion
                                });
                            }
                        }
                        else
                        {
                            error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);
                            return Ok(new
                            {
                                exitoso = false,
                                codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                mensajeError = error.Descripcion
                            });
                        }
                        //Referencia
                        salida = this._iPagoService.validarReferencia(entrada);

                        if (salida is null)
                        {
                            error = this._iPagoService.ConsultarError(Constantes.ERROR_REFERENCIA_NO_VALIDA);
                            return Ok(new
                            {
                                exitoso = false,
                                codigoError = Constantes.ERROR_REFERENCIA_NO_VALIDA,
                                mensajeError = error.Descripcion
                            });
                        }

                        mensaje = "Validación de Referencia exitosa.";
                        log.Info(mensaje);

                        return Ok(new
                        {
                            exitoso = true,
                            nombreCliente = salida.NombreCliente,
                            montoMinimo = salida.MontoMinimo,
                            montoMaximo = salida.MontoMaximo,
                            codigoError = String.Empty,
                            mensajeError = String.Empty
                        });

                    }
                    #endregion

                    #region AUTORIZAR MONTO
                    else if (entrada.Evento.Equals(Constantes.AUTORIZAR_MONTO))
                    {
                        if (entrada.MontoPago <= 0)
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }

                        //Se valida el monto y que este no contenga centavos.
                        var x = entrada.MontoPago - Math.Truncate(entrada.MontoPago);
                        if (x > 0)
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }

                        //Validar el comercio
                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
                            entrada.CajaMP = comercio.CajaMP;
                            entrada.SucursalMP = comercio.SucursalMP;
                            entrada.PlataformaMP = comercio.PlataformaMP;
                            entrada.ReferenciaMP = comercio.ReferenciaMP;
                            entrada.CveFormaPagoMP = comercio.CveFormaPagoMP;

                            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                            var body = Request.Body;
                            var todaCadena = authHeader.ToString();
                            var soloCadena = todaCadena.Substring(Constantes.BASIC_AUTH.Length, todaCadena.Length - Constantes.BASIC_AUTH.Length);

                            //Se desencripta cadena del Header y se obtienen usuario y contraseña 
                            Seguridad seg = new Seguridad();
                            Usuario usuario = seg.GetUsuario(soloCadena);

                            if (!comercio.NombreUsuario.Equals(usuario.Nombre) ||
                                !comercio.Contrasena.Equals(usuario.Contrasena))
                            {
                                return Ok(new
                                {
                                    exitoso = false
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }

                        bool esValido = this._iPagoService.EsMontoValido(entrada);
                        if (!esValido)
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                exitoso = true
                            });
                        }
                    }
                    #endregion

                    #region NOTIFICAR PAGO
                    else if (entrada.Evento.Equals(Constantes.NOTIFICAR_PAGO))
                    {

                        //Se valida el monto y que este no contenga centavos.
                        var x = entrada.MontoPago - Math.Truncate(entrada.MontoPago);
                        if (x > 0)
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false,
                            });
                        }

                        if (!(entrada.MontoPago >= 1))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });
                        }

                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
                            entrada.CajaMP = comercio.CajaMP;
                            entrada.SucursalMP = comercio.SucursalMP;
                            entrada.PlataformaMP = comercio.PlataformaMP;
                            entrada.ReferenciaMP = comercio.ReferenciaMP;
                            entrada.CveFormaPagoMP = comercio.CveFormaPagoMP;

                            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                            var body = Request.Body;
                            var todaCadena = authHeader.ToString();
                            var soloCadena = todaCadena.Substring(Constantes.BASIC_AUTH.Length, todaCadena.Length - Constantes.BASIC_AUTH.Length);

                            //Se desencripta cadena del Header y se obtienen usuario y contraseña 
                            Seguridad seg = new Seguridad();
                            Usuario usuario = seg.GetUsuario(soloCadena);

                            if (!comercio.NombreUsuario.Equals(usuario.Nombre) ||
                                !comercio.Contrasena.Equals(usuario.Contrasena))
                            {
                                return Ok(new
                                {
                                    exitoso = false,
                                    registradoAnteriormente = false
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });

                        }

                        //Verificar que trx no esté vacía ni que fecha tenga cero o menos
                        if (entrada.NumeroTransaccion.Equals(String.Empty))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });
                        }

                        //Se verifica intervalo de fecha valido
                        if (entrada.FechaHoraTransaccion <= 0)
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });
                        }

                        //Verifica si: transaccion ya existe, comercio ya existe, clave_sucursal ya existe, referencia ya existe y estatus=1.
                        //Entonces devuelve true
                        bool existeTrx = this._iPagoService.ExisteTransaccion(entrada);
                        if (existeTrx)
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = true
                            });
                        }

                        //Quizá este de más esta sección de referencia
                        salida = this._iPagoService.validarReferencia(entrada);
                        //salida es null si no encuentra datos de la referencia
                        if (salida is null)
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });
                        }
                        else
                        {
                            //if (entrada.MontoPago < salida.MontoMinimo || entrada.MontoPago > salida.MontoMaximo)
                            if (entrada.MontoPago <= 0)
                            {
                                return Ok(new
                                {
                                    exitoso = false,
                                    registradoAnteriormente = false
                                });
                            }
                        }

                        //Consultar descripcion(nombre) del comercio
                        entrada = this._iPagoService.ConsultarComercio(entrada);

                        //Se obtiene el id consecutivo de pagomp_pago
                        int idDevuelto = this._iPagoService.GrabarPago(entrada);
                        entrada.IDDevuelto = idDevuelto;

                        try
                        {
                            int generado = this._iPagoService.GenerarAbono(entrada);
                            if (generado > 0)
                            {
                                entrada.AsIDAbono = generado;
                                int filasAfectadas = this._iPagoService.ActualizarPago(entrada);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Excepción al GenerarAbono: " + ex.Message);
                            return Ok(new
                            {
                                exitoso = true,
                                registradoAnteriormente = false
                            });
                        }

                        return Ok(new
                        {
                            exitoso = true,
                            registradoAnteriormente = false
                        });

                    }

                    #endregion

                    #region VALIDAR_TRANSACCION

                    else if (entrada.Evento.Equals(Constantes.VALIDAR_TRANSACCION))
                    {
                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
                            entrada.CajaMP = comercio.CajaMP;
                            entrada.SucursalMP = comercio.SucursalMP;
                            entrada.PlataformaMP = comercio.PlataformaMP;
                            entrada.ReferenciaMP = comercio.ReferenciaMP;
                            entrada.CveFormaPagoMP = comercio.CveFormaPagoMP;

                            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                            var body = Request.Body;
                            var todaCadena = authHeader.ToString();
                            var soloCadena = todaCadena.Substring(Constantes.BASIC_AUTH.Length, todaCadena.Length - Constantes.BASIC_AUTH.Length);

                            //Se desencripta cadena del Header y se obtienen usuario y contraseña 
                            Seguridad seg = new Seguridad();
                            Usuario usuario = seg.GetUsuario(soloCadena);

                            if (!comercio.NombreUsuario.Equals(usuario.Nombre) ||
                                !comercio.Contrasena.Equals(usuario.Contrasena))
                            {
                                return Ok(new
                                {
                                    exitoso = false,
                                    existe = false
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                existe = false
                            });

                        }

                        //Validar Referencia
                        //salida = this._iPagoService.validarReferencia(entrada);

                        //if (salida is null)
                        //{
                        //    error = this._iPagoService.ConsultarError(Constantes.ERROR_REFERENCIA_NO_VALIDA);
                        //    return Ok(new
                        //    {
                        //        exitoso = false,
                        //        existe = false
                        //    });
                        //}

                        //Verifica si: transaccion ya existe, comercio ya existe, clave_sucursal ya existe, referencia ya existe y estatus=1.
                        //Entonces devuelve true
                        bool existeTrx = this._iPagoService.ExisteTransaccion(entrada);
                        if (existeTrx)
                        {
                            return Ok(new
                            {
                                exitoso = true,
                                existe = true
                            });
                        }
                        else {
                            return Ok(new
                            {
                                exitoso = true,
                                existe = false
                            });
                        }
                    }

                    #endregion
                    else
                    {
                        error = this._iPagoService.ConsultarError(Constantes.ERROR_METODO_INVALIDO);
                        return Ok(new
                        {
                            exitoso = false,
                            codigoError = Constantes.ERROR_METODO_INVALIDO,
                            mensajeError = error.Descripcion
                        });
                    }
                    
                    return Ok(new {});
                }

                return Ok(new { });
            }
            catch (Exception ex)
            {
                error = new Error();
                error.Codigo = Constantes.ERROR_GENERAL;
                error.Descripcion = "Error general";
                mensaje = error.Descripcion + ". ";
                log.Error(mensaje + ex.Message);
                return Ok(new
                {
                    exitoso = false,
                    mensaje = error.Descripcion
                });
            }
        }
    }
}
