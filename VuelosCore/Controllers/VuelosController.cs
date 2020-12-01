using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutheticationLibrary;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using VuelosCore.Data;
using VuelosCore.Data.Entities;
using VuelosCore.Interfaces;
using VuelosCore.Models.DTOs;
using VuelosCore.Models.Responses;
using VuelosCore.Services;
using static VuelosCore.Models.DTOs.Consulta4;

namespace VuelosCore.Controllers
{
    [Route("api/v1/Vuelos")]
    [ApiController]
    public class VuelosController : ControllerBase
    {
        private readonly ILogger<VuelosController> Logger;
        private readonly ApplicationDbContext _db;
        private readonly ProducerConfig _config;
        private readonly IAppLogger<ServidorCache> _loggercache;
        public VuelosController(ILogger<VuelosController> logger, ApplicationDbContext context, ProducerConfig config, IAppLogger<ServidorCache> loggercache)
        {
            Logger = logger;
            _db = context;
            _config = config;
            _loggercache = loggercache;
        }
        [HttpGet]
        [Route("GetAeropuertos")]
        [EnableCors("AllowAll")]
        public IActionResult GetAeropuertos()
        {
            try
            {
                Logger.LogInformation("Inicia obtencion de aeropuertos");
                var aeropuertos = _db.Aeropuertos.Where(c => !string.IsNullOrEmpty(c.Lata)).OrderBy(c => c.CiudadUbicacin).ToList();
                List<ResponseAeropuertos> responseAeropuertos = new List<ResponseAeropuertos>();
                foreach (var item in aeropuertos)
                {
                    if (responseAeropuertos.FirstOrDefault(c => c.CiudadUbicacion == item.CiudadUbicacin) == null)
                    {
                        responseAeropuertos.Add(new ResponseAeropuertos
                        {
                            CiudadUbicacion = item.CiudadUbicacin,
                            Iata = item.Lata,
                            Id = item.Id,
                            Concatenado = $"{item.CiudadUbicacin}[{item.Lata}]"
                        });
                    }
                }
                Logger.LogInformation(responseAeropuertos.ToString());
                return Ok(responseAeropuertos);
            }
            catch (Exception ex)
            {
                Logger.LogError("Excepcion generada en GetAeropuertos: " + ex.Message);
                return StatusCode(500, "Ocurrio un error");
                throw;
            }
        }
        [HttpPost]
        [Route("ConsultarVuelos")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> ConsultarVuelosAsync([FromBody] ConsultarVuelosDTO model)
        {
            try
            {
                DateTime dateTimeInicio;
                DateTime dateTimeFinal;
                if (!DateTime.TryParseExact(model.FechaInicio, "yyyy'-'MM'-'dd",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out dateTimeInicio))
                {
                    return BadRequest("Formato de fecha invalido, formato permitido dd/MM/aaaa");
                }
                if (!DateTime.TryParseExact(model.FechaFinal, "yyyy'-'MM'-'dd",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out dateTimeFinal))
                {
                    return BadRequest("Formato de fecha invalido, formato permitido yyyy-MM-dd");
                }
                var origen = _db.Aeropuertos.FirstOrDefault(c => c.CiudadUbicacin == model.Origen);
                if (origen == null)
                {
                    return NotFound("No se encontro la ciudad de origen");
                }
                var destino = _db.Aeropuertos.FirstOrDefault(c => c.CiudadUbicacin == model.Destino);
                if (origen == null)
                {
                    return NotFound("No se encontro la ciudad de destino");
                }
                ParametrosDTO parametros = new ParametrosDTO();
                parametros.processType = "CATALOG";
                parametros.Uuid = model.Uuid;
                parametros.Tipo_proveedor = "FLIGHTS";
                parametros.Tipo_proceso = "catalogue";
                Consulta consultaVuelos = new Consulta
                {
                    Class = "Bar",
                    Destination = destino.Lata,
                    EndDate = model.FechaFinal,
                    Origin = origen.Lata,
                    QuantityTravellers = model.CantidadPasajeros.ToString(),
                    StartDate = model.FechaInicio
                };
                parametros.Parametros.vuelos.consulta = consultaVuelos;
                string parametrosSerializados = JsonConvert.SerializeObject(parametros);
                using (var producer = new ProducerBuilder<Null, string>(_config).Build())
                {
                    await producer.ProduceAsync("topic-info-reader", new Message<Null, string>
                    {
                        Value = parametrosSerializados
                    });
                    producer.Flush(TimeSpan.FromSeconds(10));
                    return Ok();
                }                
            }
            catch (Exception ex)
            {
                Logger.LogError("Excepcion generada en ConsultarVuelos: " + ex.Message);
                return StatusCode(500, "Ocurrio un error");
                throw;
            }
        }
        [HttpPost]
        [Route("ReservarVuelo")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> ReservarVueloAsync([FromBody] ReservaDTO model, [FromHeader] string Token)
        {
            try
            {
                Logger.LogInformation("INICIA PROCESO DE RESERVA DE VUELO");
                JwtProvider jwt = new JwtProvider("TouresBalon.com", "UsuariosPlataforma");
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var first = accessToken.FirstOrDefault();
                if (string.IsNullOrEmpty(accessToken) || !first.Contains("Bearer"))
                {
                    return BadRequest();
                }
                string token = first.Replace("Bearer", "").Trim();
                Logger.LogInformation("INICIA PROCESO DE VALIDACION DE TOKEN :" + token);
                var a = jwt.ValidateToken(token);
                if (!a)
                {
                    return Unauthorized();
                }
                ParametrosDTO parametros = new ParametrosDTO();
                parametros.processType = "CATALOG";
                parametros.Uuid = model.Uuid;
                parametros.Tipo_proveedor = "FLIGHTS";
                parametros.Tipo_proceso = "catalogue";
                Reserva reserva = new Reserva
                {
                    FlightCode = model.CodigoVuelo,
                    LastName = model.Apellido,
                    Name = model.Nombre
                };
                parametros.Parametros.vuelos.reserva = reserva;
                string parametrosSerializados = JsonConvert.SerializeObject(parametros);
                using (var producer = new ProducerBuilder<Null, string>(_config).Build())
                {
                    await producer.ProduceAsync("topic-info-reader", new Message<Null, string>
                    {
                        Value = parametrosSerializados
                    });
                    producer.Flush(TimeSpan.FromSeconds(10));
                    return Ok();
                }                
            }
            catch (Exception ex)
            {
                Logger.LogError("Excepcion generada en ReservarVuelo: " + ex.Message);
                return StatusCode(500, "Ocurrio un error");
                throw;
            }
        }

        [HttpGet]
        [Route("ConsultarVuelosUiid")]
        [EnableCors("AllowAll")]
        public IActionResult ConsultarVuelosUiid(string uuid)
        {
            try
            {
                if (string.IsNullOrEmpty(uuid))
                {
                    return BadRequest();
                }
                else
                {
                    ServidorCache servidorCache = new ServidorCache(_loggercache);
                    var vuelos = servidorCache.getCache(uuid + "_FLIGHTS" + "_CATALOG");

                    if (vuelos != null)
                    {                        
                        Logger.LogInformation("Se obtiene respuesta de normalizador :" + vuelos);
                        Logger.LogInformation($"Contiene {vuelos.providersResponse.Count} proveedores la respuesta");
                        if (vuelos.providersResponse.Count > 0)
                        {
                            List<ResponseBaseVuelos> responseVuelos = new List<ResponseBaseVuelos>();
                            foreach (var item in vuelos.providersResponse)
                            {
                                Logger.LogInformation($"proveedor {item}");
                                if (!string.IsNullOrEmpty(item.code) && !string.IsNullOrEmpty(item.destination) && !string.IsNullOrEmpty(item.origin) && !string.IsNullOrEmpty(item.startDate) && !string.IsNullOrEmpty(item.endDate) && !string.IsNullOrEmpty(item.price) && !string.IsNullOrEmpty(item.providerName))
                                {
                                    Logger.LogInformation($"proveedor valido");
                                    Logger.LogInformation($"Ciudad origen {item.origin}");
                                    Logger.LogInformation($"Ciudad destino {item.destination}");
                                    var origen = _db.Aeropuertos.FirstOrDefault(c => c.Lata == item.origin);
                                    var destino = _db.Aeropuertos.FirstOrDefault(c => c.Lata == item.destination);
                                    if (origen != null && destino != null)
                                    {
                                        DateTime stardate = DateTime.Parse(item.startDate);
                                        Logger.LogInformation($"origen y destino validos");
                                        responseVuelos.Add(new ResponseBaseVuelos
                                        {
                                            OriginAirport = origen.Lata,
                                            DestinationAirport = destino.Lata,
                                            Destination = destino.CiudadUbicacin,
                                            Origin = origen.CiudadUbicacin,
                                            Stardate = DateTime.Parse(item.startDate),
                                            EndDate = DateTime.Parse(item.endDate),
                                            FligthCode = item.code,
                                            Price = item.price,
                                            Supplier = item.providerName
                                        });
                                        Logger.LogInformation($"proveedor agregado correctamente");
                                    }
                                    else
                                    {
                                        Logger.LogInformation($"origen y destino invalidos");
                                    }
                                }
                                else
                                {
                                    Logger.LogInformation($"proveedor invalido");
                                }
                            }
                            return Ok(responseVuelos);
                        }
                        else
                        {
                            return NotFound("No existen proveedores disponibles para esta solicitud");
                        }
                    }
                    else
                    {
                        return NotFound("No se encontro informacion con este Uuid");
                    }


                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Excepcion generada en ConsutlarVuelosUuid: " + ex.Message);
                return StatusCode(500, "Ocurrio un error");
                throw;
            }
        }
        [HttpGet]
        [Route("Healty")]
        public IActionResult Healty()
        {
            JwtProvider jwt = new JwtProvider("TouresBalon.com", "UsuariosPlataforma");

            var a = jwt.ValidateToken("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Ikdlcm1hbiBTaWx2YSIsInJvbGUiOiJhZG1pbiIsInByaW1hcnlzaWQiOiIxIiwiU3JjSW1nIjoiLi4vLi4vLi4vYXNzZXRzL2ltYWdlcy9Ub3VyZXNCYWxvbi9nZXJtYW4tcHJvZmlsZS5wbmciLCJuYmYiOjE2MDYwODkwODksImV4cCI6MTYwNjExNzg4OCwiaWF0IjoxNjA2MDg5MDg5LCJpc3MiOiJUb3VyZXNCYWxvbi5jb20iLCJhdWQiOiJVc3Vhcmlvc1BsYXRhZm9ybWEifQ.MkVkMot8CFnsNgvZVYbkAYJe2vXRsfdUjGjtgCs08o9oK6O9oGOeAnFooQaZYHg6T0E3p6noh2UzBmKuzv2ds4Zptm8aj5WqxP-KRmfAWgusbjdjTT4K90bRU5HpPwVzFrNUyUbJhFwb7Bjx374PB0d0AGJlA1CdAsUFQZvpP3JxvGdTLUOxFQPpa4lJRw5NZvJZJi_kO14vIc1C12V6UtKaj_SMN1FlSwp38QhylfwIokm4I8Thx7bNk2fxdZn3CMcmhzZOm5Vw92O5qUIo3ps9sMeh5l0UeuHsr11O2x05G3vDuQaU-LA09XRU_3OE2YySYtW9oU_POKAiALpoBw");
            return Ok("Todo Bien");
        }
    }
}
