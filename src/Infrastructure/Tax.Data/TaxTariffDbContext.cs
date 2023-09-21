using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data
{
    public class TaxTariffDbContext : DbContext
    {
        public DbSet<TaxTariffModel> Tariffs { get; set; }

        public TaxTariffDbContext(DbContextOptions<TaxTariffDbContext> options)
        :base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxTariffModel>().ToTable("Steuertarif");
            modelBuilder.Entity<TaxTariffModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
