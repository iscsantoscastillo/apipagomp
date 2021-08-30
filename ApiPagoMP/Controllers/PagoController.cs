using ApiPagoMP.Helpers;
using ApiPagoMP.Models;
using ApiPagoMP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public async Task<IActionResult> AsignarContrasenia([FromBody] Entrada entrada)
        {
            string mensaje = null;
            Salida salida = null;
            try
            {
                if (entrada != null)
                {
                    if (entrada.Evento.Equals(Constantes.VALIDAR_REFERENCIA))
                    {
                        salida = this._iPagoService.validarReferencia(entrada);

                        if (entrada.Referencia.Equals("SL202009000015")) {
                            return Ok(new {
                                exitoso = false,
                                nombreCliente = String.Empty,
                                montoMinimo = 0,
                                montoMaximo = 0,
                                codigoError = "01",
                                mensajeError = "La referencia está cancelada"
                            });
                        }

                        if (entrada.Referencia.Equals("SL202009000016"))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                nombreCliente = String.Empty,
                                montoMinimo = 0,
                                montoMaximo = 0,
                                codigoError = "02",
                                mensajeError = "Error general"
                            });
                        }

                        if (!entrada.Comercio.Equals("5001"))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                nombreCliente = String.Empty,
                                montoMinimo = 0,
                                montoMaximo = 0,
                                codigoError = "03",
                                mensajeError = "Comercio no autorizado"
                            });
                        }

                        if (entrada.Referencia.Equals("SL202009000017"))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                nombreCliente = String.Empty,
                                montoMinimo = 0,
                                montoMaximo = 0,
                                codigoError = "04",
                                mensajeError = "Servicio en mantenimiento"
                            });
                        }

                        if (entrada.Referencia.Equals("SL202009000018"))
                        {
                            return Ok(new
                            {
                                exitoso = false,
                                nombreCliente = String.Empty,
                                montoMinimo = 0,
                                montoMaximo = 0,
                                codigoError = "05",
                                mensajeError = "Error no definido"
                            });
                        }

                        if (entrada.Referencia.Equals("SL202009000014")) {
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
                        
                    } else if (entrada.Evento.Equals(Constantes.AUTORIZAR_MONTO))
                    {
                        if (!entrada.Referencia.Equals("SL202009000014") ||
                            !entrada.Comercio.Equals("5001")) {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }

                        salida = this._iPagoService.validarReferencia(entrada);
                        if (entrada.MontoPago >= salida.MontoMinimo && entrada.MontoPago <= salida.MontoMaximo)
                        {
                            return Ok(new
                            {
                                exitoso = true
                            });
                        }
                        else {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }
                        
                    }
                    else if (entrada.Evento.Equals(Constantes.NOTIFICAR_PAGO))
                    {
                        if (!entrada.Referencia.Equals("SL202009000014") ||
                            !entrada.Comercio.Equals("5001") ||
                            entrada.NumeroTransaccion.Equals(String.Empty) ||
                            entrada.FechaHoraTransaccion <= 0 )
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }
                        salida = this._iPagoService.validarReferencia(entrada);
                        if (entrada.MontoPago < salida.MontoMinimo || entrada.MontoPago > salida.MontoMaximo)
                        {
                            return Ok(new
                            {
                                exitoso = false
                            });
                        }

                        return Ok(new
                        {
                            exitoso = true
                        });
                    }else {
                        return Ok(new { 
                            exitoso = false,
                            nombreCliente = String.Empty,
                            montoMinimo = 0,
                            montoMaximo = 0,
                            codigoError = "06",
                            mensajeError = "Método inválido"
                        });
                    }
                    
                    return NotFound(new {});
                }

                return NotFound(new { });
            }
            catch (Exception ex)
            {
                mensaje = "Ocurrió un error inesperado al asignar contraseña";
                log.Error(mensaje + ex.Message);
                return NotFound(new
                {
                    exitoso = false,
                    mensaje = mensaje
                });
            }
        }
    }
}
