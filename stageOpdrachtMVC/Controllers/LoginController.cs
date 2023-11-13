using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace stageOpdrachtMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private const string Secret = "YourSuperSecretKeyHere"; // Gebruik een sterke sleutel in productie!

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // In de werkelijkheid controleer je hier inloggegevens in de database
            if (login.Username == "johndoe" && login.Password == "password")
            {
                var token = GenerateToken(login.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
}
