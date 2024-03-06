using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using CoreBusiness;
using SharedModels;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Domino.Models;

namespace Domino.Controllers
{
    [ApiController]
    [Route("domino")]
    public class DominoController : ControllerBase
    {
        readonly DominoService service = new DominoService();

        [HttpPost]
        [Route("fichas")]
        public IActionResult EnviarFichas([FromBody] List<Fichas> fichas)
        {
            try
            {
                // Validar el token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == null || token == string.Empty)
                {
                    return Unauthorized();
                }

                UserLogin user = UserLogin.UserBD();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;

                // Verificar si el token ha expirado
                var expiryDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(long.TryParse(tokenS?.Claims.FirstOrDefault(claim => claim.Type == "exp")?.Value, out long expiryUnixSeconds) ? expiryUnixSeconds : 0).UtcDateTime;
                if (expiryDateTimeUtc <= DateTime.UtcNow)
                {
                    return Unauthorized("Su sesión ha expirado, por favor generar nuevamente el token");
                }

                var idUserIn = tokenS.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value ?? string.Empty;
                var userIn = tokenS.Claims.FirstOrDefault(claim => claim.Type == "user")?.Value ?? string.Empty;
                var passIn = tokenS.Claims.FirstOrDefault(claim => claim.Type == "password")?.Value ?? string.Empty;

                //Si coincide el usuario entrante con el usuario registrado
                if (idUserIn == user.IdUser && userIn == user.UserName && passIn == user.Password)
                {
                    // Procesar las fichas
                    if (fichas == null || fichas.Count < 2 || fichas.Count > 6)
                    {
                        return BadRequest("El conjunto de fichas debe tener entre 2 y 6 fichas");
                    }

                    var fichasOrdenadas = service.VerificarFichas(fichas);
                    if (fichasOrdenadas == null)
                    {
                        return BadRequest("Las fichas no forman una cadena válida");
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Fichas ordenadas correctamente",
                        fichas = fichasOrdenadas
                    });
                }
                else return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al verificar las fichas. Error: " + ex.Message.ToString());
            }
           
        }
    }
}