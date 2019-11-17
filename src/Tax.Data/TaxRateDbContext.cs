using Microsoft.EntityFrameworkCore;
using Tax.Data.Abstractions.Models;

namespace Tax.Data
{
    public class TaxRateDbContext : DbContext
    {
        public DbSet<TaxRateModel> Rates { get; set; }

        public TaxRateDbContext(DbContextOptions<TaxRateDbContext> options)
        :base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRateModel>().ToTable("Steuerfuss");
            modelBuilder.Entity<TaxRateModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
