using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domino.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Domino.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        public IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        [Route("login")]
        public dynamic Login([FromBody] object optData) 
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());

                string user = data.usuario.ToString();
                string password = data.password.ToString();

                UserLogin userLogin = UserLogin.UserBD();

                if (userLogin.UserName != user || userLogin.Password != password)
                {
                    return new
                    {
                        success = false,
                        message = "Credenciales Incorrectas",
                        result = ""
                    };
                }

                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub,jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                new Claim("id", userLogin.IdUser),
                new Claim("user", userLogin.UserName),
                new Claim("password", userLogin.Password),
            };

                byte[] keyBytes;
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    var llave = new byte[32];
                    rng.GetBytes(llave);
                    keyBytes = llave;
                }

                var key = new SymmetricSecurityKey(keyBytes);
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken
                (
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(jwt.ExpiresInMinutes),
                    signingCredentials: signIn
                );

                return new
                {
                    success = true,
                    message = "Token generado exitosamente",
                    result = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error interno al verificar las fichas. Mensaje: " + ex.Message.ToString());
            }
            
        }

    }
}
