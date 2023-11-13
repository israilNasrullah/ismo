using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models.BestellingenDb;
using stageOpdrachtMVC.Models;
namespace stageOpdrachtMVC.Controllers
{
    public class BestellingenController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        public BestellingenController()
        {
            this.applicationDbContext = new ApplicationDbContext();
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Admin") == "true")
            {
                var bestelling = applicationDbContext.Bestellingens.ToList();
                return View(bestelling);
            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
            }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bestelling = applicationDbContext.Bestellingens.FirstOrDefault(x => x.Id == id);

            if(bestelling != null)
            {
                var bestellingDetail = new DetailBestellingenModel()
                {
                    Id = bestelling.Id,
                    Name = bestelling.Name,
                    Email = bestelling.Email,
                    Postcode = bestelling.Postcode,
                    Producten = bestelling.Producten,
                    TotalePrijs = bestelling.TotalePrijs,
                    Datum = bestelling.Datum,
                    Verwerkt = bestelling.Verwerkt


                };

                return await Task.Run(() => View("Details", bestellingDetail));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Details(DetailBestellingenModel model)
        {
            var bestelling = await applicationDbContext.Bestellingens.FindAsync(model.Id);
            if(bestelling != null)
            {
                bestelling.Name = model.Name;
                bestelling.Email = model.Email;
                bestelling.Postcode = model.Postcode;
                bestelling.Producten = model.Producten;
                bestelling.TotalePrijs = model.TotalePrijs;
                bestelling.Datum = model.Datum;
                bestelling.Verwerkt = model.Verwerkt;

                await applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
