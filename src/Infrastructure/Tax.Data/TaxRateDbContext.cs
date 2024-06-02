using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data
{
    public class TaxRateDbContext : DbContext
    {
        public DbSet<TaxRateEntity> Rates { get; set; }

        public TaxRateDbContext(DbContextOptions<TaxRateDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRateEntity>().ToTable("Steuerfuss");
            modelBuilder.Entity<TaxRateEntity>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
