using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ElisBackend.Gateways.Dal {

    public class ElisContext : DbContext {

        public ElisContext(DbContextOptions<ElisContext> options) : base(options) { }

        public DbSet<StockDao> Stocks { get; set; }        
        public DbSet<ExchangeDao> Exchanges { get; set; }
        public DbSet<CurrencyDao> Currencies { get; set; }
        public DbSet<TimeZoneDao> TimeZones { get; set; }
        public DbSet<DateDao> Dates { get; set; }
        public DbSet<TimeSerieDao> TimeSeries { get; set; }
        public DbSet<TimeSerieFactDao> TimeSerieFacts { get; set; }

        public DbSet<StockSearchResultDao> StockResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockDao>()                
                .HasKey(x => x.Id)                
                .HasName("StockId_PK");
            modelBuilder.Entity<StockDao>()
                .HasMany(x => x.TimeSeriesFacts)
                .WithOne(x => x.Stock);
            modelBuilder.Entity<ExchangeDao>()
                .HasKey(x => x.Id)
                .HasName("ExchangeId_PK");
            modelBuilder.Entity<ExchangeDao>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<ExchangeDao>()
                .HasMany(x => x.Stocks)
                .WithOne(x => x.Exchange);
            modelBuilder.Entity<CurrencyDao>()
                .HasKey(x => x.Id)
                .HasName("CurrencyId_PK");
            modelBuilder.Entity<CurrencyDao>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<CurrencyDao>()
                .HasMany(x => x.Stocks)
                .WithOne(x => x.Currency);
            modelBuilder.Entity<TimeZoneDao>()
                .HasKey(x => x.Id)
                .HasName("TimeZoneId_PK");
            modelBuilder.Entity<DateDao>()
                .HasKey(x => x.Id)
                .HasName("DateId_PK");
            modelBuilder.Entity<TimeSerieDao>()
                .HasKey(x => x.Id)
                .HasName("TimeSerieId_PK");
            modelBuilder.Entity<TimeSerieFactDao>()
                .HasKey( t => new { t.TimeSerieId, t.StockId, t.DateId })
                .HasName("TimeSerieFact_PK");

            modelBuilder.Entity<StockSearchResultDao>().ToTable(nameof(StockResults), t => t.ExcludeFromMigrations());
        }
    }
}
