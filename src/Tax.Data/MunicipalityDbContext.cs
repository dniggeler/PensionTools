using Microsoft.EntityFrameworkCore;
using Tax.Data.Abstractions.Models;

namespace Tax.Data
{
    public class MunicipalityDbContext : DbContext
    {
        public DbSet<MunicipalityEntity> MunicipalityEntities { get; set; }

        public MunicipalityDbContext(DbContextOptions<MunicipalityDbContext> options)
        :base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MunicipalityEntity>().ToTable("Gemeinde");
            modelBuilder.Entity<MunicipalityEntity>().HasKey(m => m.BfsNumber);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
