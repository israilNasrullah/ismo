using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using stageOpdrachtMVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace stageOpdrachtMVC.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private const string Secret = "Ik%Hoop%Dat%Deze%Code%Niet%Wordt%Gehacked";

        public static string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                return string.Empty; // or handle the case as needed
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext applicationDbContext;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger; // Injecteer de logger via de constructor
            this.applicationDbContext = new ApplicationDbContext();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] InlogAccountModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid request. Please provide both username and password.");
            }

            var hashedPassword = ComputeSha256Hash(login.Password);
            var user = applicationDbContext.Accounts.FirstOrDefault(u => u.Username == login.Username && u.Password == hashedPassword);

            if (user != null)
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

   
}

