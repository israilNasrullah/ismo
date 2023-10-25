﻿using System.Data.Entity;

namespace stageOpdrachtMVC.Models
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\demo\source\repos\stageOpdrachtConsole\db.mdf;Integrated Security=True;Connect Timeout=30")
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}