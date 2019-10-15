using System;
using Microsoft.EntityFrameworkCore;


namespace Tax.Data
{
    public class TaxRateDbContext : DbContext
    {
        public DbSet<TaxRateModel> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Filename=C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRateModel>().ToTable("Steuerfuss");
            modelBuilder.Entity<TaxRateModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
