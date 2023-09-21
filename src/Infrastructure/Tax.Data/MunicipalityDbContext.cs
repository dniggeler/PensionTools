using Domain.Models.Municipality;
using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data
{
    public class MunicipalityDbContext : DbContext
    {
        private const string StagePlzTableName = "StagePlzMunicipality";

        public DbSet<MunicipalityEntity> MunicipalityEntities { get; set; }

        public DbSet<ZipEntity> TaxMunicipalityEntities { get; set; }

        public MunicipalityDbContext(DbContextOptions<MunicipalityDbContext> options)
        :base(options)
        {}

        public int TruncateTaxMunicipalityTable()
        {
            string sql = $"delete from {StagePlzTableName};";
            return Database.ExecuteSqlRaw(sql);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MunicipalityEntity>().ToTable("HistGemeindestand");
            modelBuilder.Entity<MunicipalityEntity>().HasKey(m => new {m.BfsNumber, m.MutationId});

            modelBuilder.Entity<ZipEntity>().ToTable(StagePlzTableName);
            modelBuilder.Entity<ZipEntity>().HasKey(m => new { m.BfsNumber, m.ZipCode, m.ZipCodeAddOn });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
