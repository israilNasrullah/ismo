﻿namespace stageOpdrachtMVC.Models.Domain
{
    public class Boeken
    {
        public int id { get; set; }
        public string title { get; set; }
        public string auteur { get; set; }
        public double prijs { get; set; }
        public int publicatieJaar { get; set; }
        public int voorraad { get; set; }
    }
}
