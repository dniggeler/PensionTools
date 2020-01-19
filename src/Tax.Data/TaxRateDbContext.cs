using Microsoft.EntityFrameworkCore;
using Tax.Data.Abstractions.Models;

namespace Tax.Data
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
            modelBuilder.Entity<TaxRateEntity>().ToTable("SteuerfussZH");
            modelBuilder.Entity<TaxRateEntity>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
