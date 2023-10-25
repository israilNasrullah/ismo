using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using stageOpdrachtMVC.Models.BestellingenDb;

namespace stageOpdrachtMVC.Controllers
{
    [Route("api/Bestellingen")]
    [ApiController]
    public class BestellingenValuesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Models.Bestellingen.Bestellingen> BestellingenList()
        {
            var bestellingenInfo = new List<Models.Bestellingen.Bestellingen>();
            using(var context = new BestellingenDbContext())
            {
                bestellingenInfo = context.Bestellingens.ToList();
            }
            return bestellingenInfo;
        }
        [HttpGet("{id}")]
        public IEnumerable<Models.Bestellingen.Bestellingen> bestellingenListById(int? id)
        {
            using(var context = new BestellingenDbContext())
            {
                IQueryable<Models.Bestellingen.Bestellingen> bestellingenInfo = context.Bestellingens;
                if (id.HasValue)
                {
                    bestellingenInfo = bestellingenInfo.Where(e => e.Id == id);
                }
                return bestellingenInfo.ToList();
            }
        }

        [HttpPost]
        public async Task<bool> BestellingenInsert(AddBestellingenModel addBestellingenRequest)
        {
            try
            { 
                var totalePrijs = 0.0;

                using (var context = new BestellingenDbContext())
                {
                   
                    var boekenInfo = new List<Models.Domain.Boeken>();
                    using (var contextRead = new ApplicationDbContext())
                    {
                    string[] productenIds = addBestellingenRequest.Producten.Split("/");
                    foreach(var productenId in productenIds)
                    {
                        if (int.TryParse(productenId, out int bookId))
                        {
                                
                            var book = await contextRead.Boekens.FindAsync(bookId);
                            totalePrijs += book.prijs;
                            book.voorraad--;
                                   
                                
                            }
                    }
                        await contextRead.SaveChangesAsync();
                    }

                    DateTime currentDateTime = DateTime.Now;
                    string dateTimeString = currentDateTime.ToString("dd/MM/yyyy HH:mm");
                    var bestellingen = new Models.Bestellingen.Bestellingen()
                    {
                        Id = 0,
                        Name = addBestellingenRequest.Name,
                        Email = addBestellingenRequest.Email,
                        Postcode = addBestellingenRequest.Postcode,
                        Producten = addBestellingenRequest.Producten,
                        TotalePrijs = totalePrijs,
                        Datum = dateTimeString,
                        Verwerkt = addBestellingenRequest.Verwerkt
                    };
                    context.Bestellingens.Add(bestellingen);
                    context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
