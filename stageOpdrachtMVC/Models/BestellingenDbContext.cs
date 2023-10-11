﻿namespace stageOpdrachtMVC.Models.BestellingenDb
{using System;
using System.Configuration;
using System.Data.Entity;
using stageOpdrachtMVC.Models.Bestellingen;
    public class BestellingenDbContext : DbContext
    {
        public BestellingenDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\demo\source\repos\stageOpdrachtConsole\db.mdf;Integrated Security=True;Connect Timeout=30")
        {
        }
    public DbSet<Bestellingen> Bestellingen { get; set; }
        
    }

}