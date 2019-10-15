using System;
using Microsoft.EntityFrameworkCore;


namespace Tax.Data
{
    public class TaxRateDbContext : DbContext
    {
        public DbSet<TaxRateModel> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Filename=C:\Users\dnigg\OneDrive\temp\PensionTools\TaxDb");

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
