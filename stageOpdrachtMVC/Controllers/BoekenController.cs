
using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using Microsoft.EntityFrameworkCore;
using stageOpdrachtMVC.Models.Domain;
using stageOpdrachtMVC.Models.BestellingenDb;
using System.Net.Http.Json;

namespace stageOpdrachtMVC.Controllers
{
    public class BoekenController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly BestellingenDbContext bestellingenDbContext;
        
        public BoekenController()
        {
            this.applicationDbContext = new ApplicationDbContext();
            this.bestellingenDbContext = new BestellingenDbContext();
        }

        private string _apiUrl = "https://localhost:7118/api/Boeken";
        private List<Models.Domain.Boeken> GetFromAPI()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync("https://localhost:7118/api/Boeken/").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<List<Models.Domain.Boeken>>().GetAwaiter().GetResult();
            }
            return null;
        }

        

        [HttpGet]
        public IActionResult Index()
        {


            // var   boekenList = applicationDbContext.Boeken.ToList();
            var boekenList = GetFromAPI();
            var bestellingenModel = new AddBestellingenModel(); // Creëer een nieuw exemplaar van AddBestellingenModel.

            var viewModel = new CombinedViewModel
            {
                BoekenList = boekenList,
                BestellingenModel = bestellingenModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddBestellingenModel addBestellingenRequest)
        {
            
            var bestellingen = new Models.Bestellingen.Bestellingen()
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

           
            bestellingenDbContext.Bestellingen.Add(bestellingen);
            await bestellingenDbContext.SaveChangesAsync();

            
            string[] productIds = addBestellingenRequest.Producten.Split('/');

           
            foreach (var productId in productIds)
            {
                if (int.TryParse(productId, out int bookId))
                {
                    var book = await applicationDbContext.Boeken.FindAsync(bookId);
                    book.voorraad--;  
                }
            }

      
            await applicationDbContext.SaveChangesAsync();

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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBoekenModel addBoekenRequest)
        {
            var Boeken = new Models.Domain.Boeken()
            {
                id = 0,
                title = addBoekenRequest.title,
                auteur = addBoekenRequest.auteur,
                prijs = addBoekenRequest.prijs,
                publicatieJaar = addBoekenRequest.publicatieJaar,
                voorraad = addBoekenRequest.voorraad
            };

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsJsonAsync<Models.Domain.Boeken>(_apiUrl, Boeken).GetAwaiter().GetResult();
            
            // AddByAPI(Boeken);
            //  applicationDbContext.Boeken.Add(Boeken);
            // await applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var boek = applicationDbContext.Boeken.FirstOrDefault(x => x.id == id);

            if(boek != null)
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
                return await Task.Run(() => View("Edit", editModel));
            }

            return RedirectToAction("Index"); 
        }

        

        [HttpPost]
        public async Task<IActionResult> Edit(EditBoekenModel model)
        {
            
            //var boek = await applicationDbContext.Boeken.FindAsync(model.id); 
            HttpClient client = new HttpClient();
            string apiURL = $"https://localhost:7118/api/Boeken/{model.id}";

            var boek = new Models.Domain.Boeken
            {
                id = model.id,
                title = model.title,
                auteur = model.auteur,
                prijs = model.prijs,
                publicatieJaar = model.publicatieJaar,
                voorraad = model.voorraad
            };

            HttpResponseMessage response = await client.PutAsJsonAsync(apiURL, boek);


         /*   if (boek != null)
            {
                boek.title = model.title;   
                boek.auteur = model.auteur; 
                boek.prijs = model.prijs;
                boek.publicatieJaar = model.publicatieJaar;
                boek.voorraad = model.voorraad;

                await applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            } */
            return RedirectToAction("Index");
        }

        private void DeleteByAPI(EditBoekenModel model)
        {
            HttpClient client = new HttpClient();
            string apiURL = $"https://localhost:7118/api/Boeken/{model.id}";
            HttpResponseMessage response = client.DeleteAsync(apiURL).GetAwaiter().GetResult();
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBoekenModel model)
        {
           /* var boek = await applicationDbContext.Boeken.FindAsync(model.id);
            if(boek != null)
            {
                applicationDbContext.Boeken.Remove(boek);
                await applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            } */
           DeleteByAPI(model);

            return RedirectToAction("Index");
        }

    }


}
