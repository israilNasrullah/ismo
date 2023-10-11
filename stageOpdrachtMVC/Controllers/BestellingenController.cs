using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models.BestellingenDb;
using stageOpdrachtMVC.Models.Bestellingen;
using stageOpdrachtMVC.Models;
namespace stageOpdrachtMVC.Controllers
{
    public class BestellingenController : Controller
    {
        private readonly BestellingenDbContext bestellingenDbContext;
        public BestellingenController()
        {
            this.bestellingenDbContext = new BestellingenDbContext();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var bestelling = bestellingenDbContext.Bestellingen.ToList();
            return View(bestelling);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bestelling = bestellingenDbContext.Bestellingen.FirstOrDefault(x => x.Id == id);

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
            var bestelling = await bestellingenDbContext.Bestellingen.FindAsync(model.Id);
            if(bestelling != null)
            {
                bestelling.Name = model.Name;
                bestelling.Email = model.Email;
                bestelling.Postcode = model.Postcode;
                bestelling.Producten = model.Producten;
                bestelling.TotalePrijs = model.TotalePrijs;
                bestelling.Datum = model.Datum;
                bestelling.Verwerkt = model.Verwerkt;

                await bestellingenDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
