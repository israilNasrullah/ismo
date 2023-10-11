namespace stageOpdrachtMVC.Models.Bestellingen
{
    public class Bestellingen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }  
        public string Postcode { get; set; }
        public string Producten { get; set; }
        public double TotalePrijs { get; set; }
        public string Datum { get; set; }
        public bool Verwerkt { get; set; }


    }
}
