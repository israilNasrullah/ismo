using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using System.Diagnostics;
using stageOpdrachtMVC.Models;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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

        private readonly AccountDbContext accountDbContext;
        public HomeController()
        {
            this.accountDbContext = new AccountDbContext();
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

            accountDbContext.Accounts.Add(accounts);
            await accountDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(InlogAccountModel loginModel)
        {
            // Voer validatie van gebruikersinvoer uit indien nodig.

            var hashedPassword = ComputeSha256Hash(loginModel.Password);
            var user = accountDbContext.Accounts.FirstOrDefault(u => u.Username == loginModel.Username && u.Password == loginModel.Password);

            if (user != null)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // Voeg eventuele andere claims toe op basis van de gebruikersrol of andere informatie.
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Hiermee blijft de gebruiker ingelogd, tenzij ze zichzelf uitloggen.
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("SecurePage");
            }

            // Als de inloggegevens onjuist zijn, toon een foutmelding.
            ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
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