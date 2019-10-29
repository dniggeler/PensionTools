using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tax.Data.Abstractions.Models;

namespace Tax.Data
{
    public class FederalTaxTariffDbContext : DbContext
    {
        private readonly DbSettings _settings;

        public DbSet<FederalTaxTariffModel> Tariffs { get; set; }

        public FederalTaxTariffDbContext(IOptions<DbSettings> options)
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
            modelBuilder.Entity<FederalTaxTariffModel>().ToTable("Bundestarif");
            modelBuilder.Entity<FederalTaxTariffModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
