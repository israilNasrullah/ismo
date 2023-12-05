using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stageOpdrachtMVC.Models;
using System.Collections.Generic;


namespace stageOpdrachtMVC.Controllers
{

    [Route("api/Boeken")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<Boeken> BoekenList()
        {
            var boekenInfo = new List<Boeken>();
            using (var context = new ApplicationDbContext())
            {
                boekenInfo = context.Boekens.ToList();
            }
            return boekenInfo;
        }
        [HttpGet("{id}")]
        public IEnumerable<Boeken> BoekenListById(int? id)
        {
            using (var context = new ApplicationDbContext())
            {
                IQueryable<Boeken> boekenInfo = context.Boekens;

                if (id.HasValue)
                {
                    boekenInfo = boekenInfo.Where(e => e.id == id);
                }

                return boekenInfo.ToList();
            }
        }
        [HttpDelete("{id}")]
        public bool BoekenDelete(int? id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var boekenInfo = context.Boekens.Where(e => e.id == id).FirstOrDefault();
                    context.Boekens.Remove(boekenInfo);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public bool BoekenInsert(AddBoekenModel addBoekenRequest)
        {
            try
            {
                using (var context = new ApplicationDbContext())
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
                    context.Boekens.Add(Boeken);
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPut("{id}")]
        public bool BoekenUpdate(int? id, EditBoekenModel model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var boeken = context.Boekens.FirstOrDefault(e => e.id == id);

                    boeken.title = model.title;
                    boeken.auteur = model.auteur;
                    boeken.prijs = model.prijs;
                    boeken.publicatieJaar = model.publicatieJaar;
                    boeken.voorraad = model.voorraad;

                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }


}

