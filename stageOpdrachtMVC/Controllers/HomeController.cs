using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using System.Diagnostics;
using stageOpdrachtMVC.Models;
using System.Text;
using System.Security.Cryptography;

namespace stageOpdrachtMVC.Controllers
{
    public class HomeController : Controller
    {
        public static string ComputeSha256Hash(string rawData)
        {
            
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

        public IActionResult Index()
        {
            return View();
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