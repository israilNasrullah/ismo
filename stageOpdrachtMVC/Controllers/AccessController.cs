using Microsoft.AspNetCore.Mvc;

namespace stageOpdrachtMVC.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
