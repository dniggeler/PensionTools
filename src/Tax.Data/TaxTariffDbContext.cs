using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tax.Data.Abstractions.Models;
using Tax.Data.Models;


namespace Tax.Data
{
    public class TaxTariffDbContext : DbContext
    {
        private readonly DbSettings _settings;

        public DbSet<TaxTariffModel> Tariffs { get; set; }

        public TaxTariffDbContext(IOptions<DbSettings> options)
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
            modelBuilder.Entity<TaxTariffModel>().ToTable("Steuertarif");
            modelBuilder.Entity<TaxTariffModel>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
