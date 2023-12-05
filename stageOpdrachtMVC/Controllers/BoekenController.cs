
using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using Microsoft.EntityFrameworkCore;

using stageOpdrachtMVC.Models.BestellingenDb;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using stageOpdrachtMVC.Repositories;
using stageOpdrachtMVC.Filters;

namespace stageOpdrachtMVC.Controllers
{
   //[UserAuthorizationFilter]
    [AuthorizationFilter("admin;user")]
    public class BoekenController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BoekenController> _logger;
        private readonly IRepository<Boeken> _boekenRepository;
        private readonly IRepository<Bestellingen> _bestellingenRepository;
       

        public BoekenController(ILogger<BoekenController> logger, IRepository<Boeken> boekenRepository, IRepository<Bestellingen> bestellingenRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _boekenRepository = boekenRepository;
            _bestellingenRepository = bestellingenRepository;
            

        }


        private string _apiUrl = "https://localhost:7118/api/Boeken";


        private List<Boeken> GetFromAPI()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync("https://localhost:7118/api/Boeken/").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<List<Boeken>>().GetAwaiter().GetResult();
            }
            return null;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var boekenList = _boekenRepository.GetAll();
                var bestellingenModel = new AddBestellingenModel();

                var viewModel = new CombinedViewModel
                {
                    BoekenList = boekenList.ToList(),
                    BestellingenModel = bestellingenModel
                };
            

                return View(viewModel);
        }





        [HttpPost]
        public async Task<IActionResult> Index(AddBestellingenModel addBestellingenRequest)
        {
           // HttpContext.User.HasClaim
            var bestellingen = new Bestellingen()
            {
                Id = 0,
                Name = addBestellingenRequest.Name,
                Email = addBestellingenRequest.Email,
                Postcode = addBestellingenRequest.Postcode,
                Producten = addBestellingenRequest.Producten,
                TotalePrijs = addBestellingenRequest.TotalePrijs,
                Datum = addBestellingenRequest.Datum,
                Verwerkt = addBestellingenRequest.Verwerkt
            };

            _bestellingenRepository.Add(bestellingen);
            await _bestellingenRepository.SaveChangesAsync();

            string[] productIds = addBestellingenRequest.Producten.Split('/');

            foreach (var productId in productIds)
            {
                if (int.TryParse(productId, out int bookId))
                {
                    var book = await _boekenRepository.GetByIdAsync(bookId);
                    if (book != null)
                    {
                        book.voorraad--;
                        _boekenRepository.Update(book);
                    }
                }
            }

            await _boekenRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        /*      private Models.Domain.Boeken AddByAPI(Models.Domain.Boeken boek)
              {
                  HttpClient client = new HttpClient();
                  HttpResponseMessage response = client.PostAsJsonAsync<Models.Domain.Boeken>(_apiUrl, boek).GetAwaiter().GetResult();
                  if (response.IsSuccessStatusCode)
                  {
                      return response.Content.ReadFromJsonAsync<Models.Domain.Boeken>().GetAwaiter().GetResult();
                  }
                  return null;
              }  */

        [AuthorizationFilter("admin")]
        [HttpGet]
        public IActionResult Add()
        {
           
            return View();
        }
        [AuthorizationFilter("admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AddBoekenModel addBoekenRequest)
        {

            var Boeken = new Boeken()
            {
                id = 0,
                title = addBoekenRequest.title,
                auteur = addBoekenRequest.auteur,
                prijs = addBoekenRequest.prijs,
                publicatieJaar = addBoekenRequest.publicatieJaar,
                voorraad = addBoekenRequest.voorraad
            };

            //HttpClient client = new HttpClient();
            //HttpResponseMessage response = client.PostAsJsonAsync<Boeken>(_apiUrl, Boeken).GetAwaiter().GetResult();


            _boekenRepository.Add(Boeken);
            await _boekenRepository.SaveChangesAsync();

            return RedirectToAction("Index");


        }


        [AuthorizationFilter("admin")]
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [AuthorizationFilter("admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var boek = await _boekenRepository.GetByIdAsync(id);

            if (boek != null)
            {
                var editModel = new EditBoekenModel()
                {
                    id = boek.id,
                    title = boek.title,
                    auteur = boek.auteur,
                    prijs = boek.prijs,
                    publicatieJaar = boek.publicatieJaar,
                    voorraad = boek.voorraad
                };
                return View("Edit", editModel);
            }

            return RedirectToAction("Index");


        }



        [AuthorizationFilter("admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBoekenModel model)
        {
            //HttpClient client = new HttpClient();
            //string apiURL = $"https://localhost:7118/api/Boeken/{model.id}";

            //var boek = new Boeken
            //{
            //    id = model.id,
            //    title = model.title,
            //    auteur = model.auteur,
            //    prijs = model.prijs,
            //    publicatieJaar = model.publicatieJaar,
            //    voorraad = model.voorraad
            //};

            //  HttpResponseMessage response = await client.PutAsJsonAsync(apiURL, boek);

            var boek = await _boekenRepository.GetByIdAsync(model.id);

            if (boek != null)
            {
                boek.title = model.title;
                boek.auteur = model.auteur;
                boek.prijs = model.prijs;
                boek.publicatieJaar = model.publicatieJaar;
                boek.voorraad = model.voorraad;

                await _boekenRepository.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private void DeleteByAPI(EditBoekenModel model)
        {
            HttpClient client = new HttpClient();
            string apiURL = $"https://localhost:7118/api/Boeken/{model.id}";
            HttpResponseMessage response = client.DeleteAsync(apiURL).GetAwaiter().GetResult();

        }
        [AuthorizationFilter("admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EditBoekenModel model)
        {
            var boek = await _boekenRepository.GetByIdAsync(model.id);
            if (boek != null)
            {
                _boekenRepository.Remove(boek);
                await _boekenRepository.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            //DeleteByAPI(model);

            return RedirectToAction("Index");
        }

    }


}
