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
            modelBuilder.Entity<MunicipalityEntity>().ToTable("HistGemeindestand");
            modelBuilder.Entity<MunicipalityEntity>().HasKey(m => new {m.BfsNumber, m.MutationId});
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
