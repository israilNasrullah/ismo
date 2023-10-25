namespace stageOpdrachtMVC.Models
{
    using Microsoft.EntityFrameworkCore;
    using stageOpdrachtMVC.Models.Domain;
    using System;
    using System.Configuration;
    
    public class ApplicationDbContext : DbContext
    {
        static ApplicationDbContext()
        {

        }

        public ApplicationDbContext()  //: base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\demo\source\repos\stageOpdrachtConsole\db.mdf;Integrated Security=True;Connect Timeout=30")
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\demo\source\repos\stageOpdrachtConsole\db.mdf;Integrated Security=True;Connect Timeout=30");
        }

        public DbSet<Boeken> Boeken { get; set; }
        public object Models { get; internal set; }


        //public DbSet<Bestellingen> Bestellingens { get; set; }

    }

}
