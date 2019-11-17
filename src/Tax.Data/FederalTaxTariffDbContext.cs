using Microsoft.EntityFrameworkCore;
using Tax.Data.Abstractions.Models;

namespace Tax.Data
{
    public class FederalTaxTariffDbContext : DbContext
    {
        public DbSet<FederalTaxTariffModel> Tariffs { get; set; }

        public FederalTaxTariffDbContext(DbContextOptions<FederalTaxTariffDbContext> options)
        :base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FederalTaxTariffModel>().ToTable("Bundestarif");
            modelBuilder.Entity<FederalTaxTariffModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
