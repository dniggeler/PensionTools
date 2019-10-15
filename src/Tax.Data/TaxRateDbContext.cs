using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tax.Data.Abstractions.Models;


namespace Tax.Data
{
    public class TaxRateDbContext : DbContext
    {
        private readonly DbSettings _settings;

        public DbSet<TaxRateModel> Blogs { get; set; }

        public TaxRateDbContext(IOptions<DbSettings> options)
        {
            _settings = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_settings.FilePath}");

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
