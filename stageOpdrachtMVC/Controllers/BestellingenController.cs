using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Repositories;
using stageOpdrachtMVC.Models;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using stageOpdrachtMVC.Filters;

namespace stageOpdrachtMVC.Controllers
{
    [AuthorizationFilter("admin")]
    public class BestellingenController : Controller
    {
        private readonly ILogger<BestellingenController> _logger;
        private readonly IRepository<Bestellingen> _bestellingenRepository;

        public BestellingenController(ILogger<BestellingenController> logger ,IRepository<Bestellingen> bestellingenRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bestellingenRepository = bestellingenRepository;
        }
       
        [HttpGet]
        public IActionResult Index()
        {
                var bestellingen = _bestellingenRepository.GetAll();
                return View(bestellingen);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bestelling = _bestellingenRepository.GetById(id);

            if (bestelling != null)
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

                return View("Details", bestellingDetail);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Details(DetailBestellingenModel model)
        {
            var bestelling = _bestellingenRepository.GetById(model.Id);
            
            if (bestelling != null)
            {
                bestelling.Name = model.Name;
                bestelling.Email = model.Email;
                bestelling.Postcode = model.Postcode;
                bestelling.Producten = model.Producten;
                bestelling.TotalePrijs = model.TotalePrijs;
                bestelling.Datum = model.Datum;
                bestelling.Verwerkt = model.Verwerkt;

                _bestellingenRepository.Update(bestelling);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
