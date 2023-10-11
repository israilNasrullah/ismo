namespace stageOpdrachtMVC.Models
{
    using stageOpdrachtMVC.Models.Domain;
    using System;
    using System.Configuration;
    using System.Data.Entity;

    public class ApplicationDbContext : DbContext
    {
        static ApplicationDbContext()
        {

        }

        public ApplicationDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\demo\source\repos\stageOpdrachtConsole\db.mdf;Integrated Security=True;Connect Timeout=30")
        {
        }
        public DbSet<Boeken> Boeken { get; set; }
        public object Models { get; internal set; }


        //public DbSet<Bestellingen> Bestellingens { get; set; }

    }

}
