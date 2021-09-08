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
        //[ApiExplorerSettings(IgnoreApi = true)]
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

                        if (salida is null) {
                            error = this._iPagoService.ConsultarError(Constantes.ERROR_REFERENCIA_NO_VALIDA);
                            return Ok(new
                            {
                                exitoso = false,
                                codigoError = Constantes.ERROR_REFERENCIA_NO_VALIDA,
                                mensajeError = error.Descripcion
                            }) ;
                        }

                        //if (entrada.Referencia.Equals("SL202009000016"))
                        //{
                        //    return Ok(new
                        //    {
                        //        exitoso = false,                                
                        //        codigoError = "02",
                        //        mensajeError = "Error general"
                        //    });
                        //}

                        //if (!entrada.Comercio.Equals("5001"))
                        //{
                        //    return Ok(new
                        //    {
                        //        exitoso = false,                                
                        //        codigoError = "03",
                        //        mensajeError = "Comercio no autorizado"
                        //    });
                        //}

                        //if (entrada.Referencia.Equals("SL202009000017"))
                        //{
                        //    return Ok(new
                        //    {
                        //        exitoso = false,                                
                        //        codigoError = "04",
                        //        mensajeError = "Servicio en mantenimiento"
                        //    });
                        //}

                        //if (entrada.Referencia.Equals("SL202009000018"))
                        //{
                        //    return Ok(new
                        //    {
                        //        exitoso = false,                               
                        //        codigoError = "05",
                        //        mensajeError = "Error no definido"
                        //    });
                        //}                        
                            
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
                        //Se valida el monto y que este no contenga centavos.
                        var x = entrada.MontoPago - Math.Truncate(entrada.MontoPago);
                        if (x > 0) {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }
                        
                        //Validar el comercio
                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
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

                                //error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);

                                return Ok(new
                                {
                                    exitoso = false
                                    //codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                    //mensajeError = error.Descripcion
                                });
                            }
                        }else
                        {
                            //error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);
                            return Ok(new
                            {
                                exitoso = false,
                                //codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                //mensajeError = error.Descripcion
                            });
                        }


                        //if (!entrada.Referencia.Equals("SL202009000014") ||
                        //    !entrada.Comercio.Equals("5001")) {
                        //    return Ok(new
                        //    {
                        //        exitoso = false
                        //    });
                        //}

                        salida = this._iPagoService.validarReferencia(entrada);
                        if (salida is null)
                        {
                            //error = this._iPagoService.ConsultarError(Constantes.ERROR_REFERENCIA_NO_VALIDA);
                            return Ok(new
                            {
                                exitoso = false,
                                //codigoError = Constantes.ERROR_REFERENCIA_NO_VALIDA,
                                //mensajeError = error.Descripcion
                            });
                        }
                        else {
                            if (entrada.MontoPago >= salida.MontoMinimo && entrada.MontoPago <= salida.MontoMaximo)
                            {
                                return Ok(new
                                {
                                    exitoso = true
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    exitoso = false
                                });
                            }
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

                        Comercio comercio = this._iPagoService.GetComercio(entrada.Comercio);
                        if (comercio != null)
                        {
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

                                //error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);

                                return Ok(new
                                {
                                    exitoso = false,
                                    registradoAnteriormente = false,
                                    //codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                    //mensajeError = error.Descripcion
                                });
                            }
                        }
                        else
                        {
                            //error = this._iPagoService.ConsultarError(Constantes.ERROR_COMERCIO_NO_AUTORIZADO);                            
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false,
                                //codigoError = Constantes.ERROR_COMERCIO_NO_AUTORIZADO,
                                //mensajeError = error.Descripcion
                            });

                        }
                        


                        if (entrada.NumeroTransaccion.Equals(String.Empty) ||
                            entrada.FechaHoraTransaccion <= 0)
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = false
                            });
                        }

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
                        else {
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
                        
                        //Se verifica si existeAnteriormente
                        bool existe = this._iPagoService.ExisteNotiPago(entrada);
                        if (existe) {
                            return Ok(new
                            {
                                exitoso = false,
                                registradoAnteriormente = true
                            });
                        }

                        bool grabo = this._iPagoService.GrabarPago(entrada);
                        return Ok(new
                        {
                            exitoso = true,
                            registradoAnteriormente = false
                        });
                    }
                    #endregion
                    
                    else {
                        error = this._iPagoService.ConsultarError(Constantes.ERROR_METODO_INVALIDO);
                        return Ok(new { 
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
