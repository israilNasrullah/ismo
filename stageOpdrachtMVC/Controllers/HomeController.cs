using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using System.Diagnostics;
using stageOpdrachtMVC.Models;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using stageOpdrachtMVC.Repositories;

namespace stageOpdrachtMVC.Controllers
{
    public class HomeController : Controller
    {
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

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IRepository<Account> _accountRepository;
        public HomeController(ILogger<HomeController> logger, IRepository<Account> accountRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.applicationDbContext = new ApplicationDbContext();
            _accountRepository = accountRepository;
        }
     
        [HttpGet]
        public IActionResult NewAccount()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> NewAccount(AddAccountModel addAccountRequest)
        {
            var accounts = new Account()
            {
                Id = 0,
                Username = addAccountRequest.Username,
                Email = addAccountRequest.Email,
                Password = ComputeSha256Hash(addAccountRequest.Password),
                Admin = false 

            };

            _accountRepository.Add(accounts);
            await _accountRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            //  var (success, username, role) = SecurityCheck(); 
            // var username = HttpContext.User.Identity.Name;
            //ViewBag.Username = username;
            // ViewBag.Status = !string.IsNullOrEmpty(username);

                    ViewBag.Status = false;
            var model = new InlogAccountModel() { returnUrl = Request.Query["returnUrl"] };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InlogAccountModel loginModel)
        {
            var returnUrl = loginModel.returnUrl;
            var hashedPassword = ComputeSha256Hash(loginModel.Password);
            var user = applicationDbContext.Accounts.FirstOrDefault(u => u.Username == loginModel.Username && u.Password == hashedPassword);
            var role = "";
            if (user != null)
            {
                try
                {
                    if (user.Admin == true)
                    {
                        role = "admin";
                    }
                    else
                    {
                        role = "user";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Foutmelding = "Er is iets fout gegaan bij het ophalen van de rol!";
                    return RedirectToAction("Index");
                }

                string token = GenerateJwtToken(loginModel.Username, role);
                _logger.LogInformation($"Received Token: {token}");

                Response.Cookies.Append("token", token);

                _logger.LogInformation("User logged in successfully.");

                //       return RedirectToAction(returnUrl);
                return Redirect(returnUrl);
           }
           
           _logger.LogInformation("Gegevens kloppen niet!!");
            ViewBag.Foutmelding = "De gegevens kloppen niet!";
            RedirectToAction("Index");

            return View();
            
        }



        private string GenerateJwtToken(string username, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mijnkey123456789012345678901234567890"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim("Role", role)

    };

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7139/",
                audience: "https://localhost:7139/",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }




        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Markeer de gebruiker als uitgelogd in de sessie
            HttpContext.Session.Remove("role");
            Response.Cookies.Delete("token");
            return RedirectToAction("Index", "Boeken");
        }


        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}