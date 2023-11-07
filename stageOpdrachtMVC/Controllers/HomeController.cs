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
        private readonly AccountDbContext accountDbContext;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; // Injecteer de logger via de constructor
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
            
                var hashedPassword = ComputeSha256Hash(loginModel.Password);
                var user = accountDbContext.Accounts.FirstOrDefault(u => u.Username == loginModel.Username && u.Password == hashedPassword);
                

                if (user != null)
                {
                    // Markeer de gebruiker als ingelogd in de sessie
                    HttpContext.Session.SetString("Ingelogd", "true");

                if (user.Admin)
                {
                    HttpContext.Session.SetString("Admin", "true");
                    
                }
                else
                {
                    HttpContext.Session.SetString("Admin", "false");
                    
                }

                    return RedirectToAction("Index", "Boeken");
                }

                // Als inloggegevens onjuist zijn, toon een foutmelding
                ViewBag.Foutmelding = "Ongeldige inloggegevens.";
            
                return View();
            
            
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Markeer de gebruiker als uitgelogd in de sessie
            HttpContext.Session.SetString("Ingelogd", "false");
            HttpContext.Session.SetString("Admin", "false");

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