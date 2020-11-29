using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutheticationLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using VuelosCore.Data;
using VuelosCore.Data.Entities;
using VuelosCore.Models.DTOs;
using VuelosCore.Models.Responses;
using static VuelosCore.Models.DTOs.Consulta4;

namespace VuelosCore.Controllers
{
    [Route("api/v1/Vuelos")]
    [ApiController]
    public class VuelosController : ControllerBase
    {
        private readonly ILogger<VuelosController> Logger;
        private readonly ApplicationDbContext _db;
        public VuelosController(ILogger<VuelosController> logger, ApplicationDbContext context)
        {
            Logger = logger;
            _db = context;
        }
        [HttpGet]
        [Route("GetAeropuertos")]
        public IActionResult GetAeropuertos()
        {
            try
            {
                Logger.LogInformation("Inicia obtencion de aeropuertos");
                var aeropuertos = _db.Aeropuertos.Where(c => !string.IsNullOrEmpty(c.Lata)).ToList();
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
        public IActionResult ConsultarVuelos([FromBody] ConsultarVuelosDTO model)
        {
            try
            {
                DateTime dateTimeInicio;
                DateTime dateTimeFinal;
                if (!DateTime.TryParseExact(model.FechaInicio, "dd'/'MM'/'yyyy",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out dateTimeInicio))
                {
                    return BadRequest("Formato de fecha invalido, formato permitido dd/MM/aaaa");
                }
                if (!DateTime.TryParseExact(model.FechaFinal, "dd'/'MM'/'yyyy",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out dateTimeFinal))
                {
                    return BadRequest("Formato de fecha invalido, formato permitido dd/MM/aaaa");
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
                Consulta consultaVuelos = new Consulta
                {                    
                    Class = "Bar",
                    Destination = model.Destino,
                    EndDate = model.FechaFinal,
                    Origin = model.Origen,
                    QuantityTravellers = model.CantidadPasajeros.ToString(),
                    StartDate = model.FechaInicio
                };
                parametros.parameters.vuelos.consulta = consultaVuelos;              
                ResponseConsultaVuelos response = new ResponseConsultaVuelos();
                List<ResponseBase> vuelos = new List<ResponseBase>
                {
                    new ResponseBase
                    {
                        DestinationAirport=destino.Lata,
                        OriginAirport=destino.Lata,
                        Supplier="Avianca",
                        Origin = model.Origen,
                        Destination = model.Destino,
                        Stardate = DateTime.Now.AddDays(1),
                        EndDate = DateTime.Now.AddDays(5),
                        FligthCode = "41asd81asd9",
                        Price = 1000000
                    },
                    new ResponseBase
                    {
                        DestinationAirport=destino.Lata,
                        OriginAirport=destino.Lata,
                        Supplier="Avianca",
                        Origin = model.Origen,
                        Destination = model.Destino,
                        Stardate = DateTime.Now.AddDays(1),
                        EndDate = DateTime.Now.AddDays(5),
                        FligthCode = "78981asd9",
                        Price = 2000000
                    },
                    new ResponseBase
                    {
                        DestinationAirport=destino.Lata,
                        OriginAirport=destino.Lata,
                        Supplier="Avianca",
                        Origin = model.Origen,
                        Destination = model.Destino,
                        Stardate = DateTime.Now.AddDays(3),
                        EndDate = DateTime.Now.AddDays(10),
                        FligthCode = "4848151asd9",
                        Price = 3000000
                    },
                    new ResponseBase
                    {
                        DestinationAirport=destino.Lata,
                        OriginAirport=destino.Lata,
                        Supplier="Avianca",
                        Origin = model.Origen,
                        Destination = model.Destino,
                        Stardate = DateTime.Now.AddDays(5),
                        EndDate = DateTime.Now.AddDays(10),
                        FligthCode = "49891881asd9",
                        Price = 4000000
                    }
                };
                response.vuelos = vuelos;
                return Ok(response);
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
        public IActionResult ReservarVuelo([FromBody] ReservaDTO model, [FromHeader] string Token)
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
                Reserva reserva = new Reserva
                {
                    FlightCode = model.CodigoVuelo,
                    LastName = model.Apellido,
                    Name = model.Nombre
                };
                parametros.parameters.vuelos.reserva = reserva;

                _db.ReservaVuelos.Add(new ReservaVuelo
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Token = token,
                    CodigoVuelo = model.CodigoVuelo
                });
                _db.SaveChanges();
                return Ok(new ResponseReservaVuelo
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("Excepcion generada en ReservarVuelo: " + ex.Message);
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
