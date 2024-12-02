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
        public DbSet<DateDao> Dates { get; set; }
        public DbSet<TimeSeriesDao> TimeSeries { get; set; }
        public DbSet<TimeSeriesFactDao> TimeSerieFacts { get; set; }

        public DbSet<StockSearchResultDao> StockResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockDao>()                
                .HasKey(x => x.Id)                
                .HasName("StockId_PK");
            modelBuilder.Entity<StockDao>()
                .HasIndex(x => x.Isin)
                .IsUnique();
            modelBuilder.Entity<StockDao>()
                .HasMany(s => s.TimeSeries)
                .WithOne(t => t.Stock)
                .HasForeignKey(t => t.StockId)
                .IsRequired();
            modelBuilder.Entity<ExchangeDao>()
                .HasKey(e => e.Id)
                .HasName("ExchangeId_PK");
            modelBuilder.Entity<ExchangeDao>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<ExchangeDao>()
                .HasMany(e => e.Stocks)
                .WithOne(s => s.Exchange)
                .HasForeignKey(s => s.ExchangeId)
                .IsRequired();
            modelBuilder.Entity<CurrencyDao>()
                .HasKey(c => c.Id)
                .HasName("CurrencyId_PK");
            modelBuilder.Entity<CurrencyDao>()
                .HasIndex(c => c.Code)
                .IsUnique();
            modelBuilder.Entity<CurrencyDao>()
                .HasMany(c => c.Stocks)
                .WithOne(s => s.Currency)
                .HasForeignKey(s => s.CurrencyId)
                .IsRequired();
            modelBuilder.Entity<DateDao>()
                .HasKey(d => d.Id)
                .HasName("DateId_PK");
            modelBuilder.Entity<DateDao>()
                .HasIndex(d => d.DateTimeUtc)
                .IsUnique();
            modelBuilder.Entity<DateDao>()
                .HasMany(d => d.Facts)
                .WithOne(f => f.Date)
                .HasForeignKey(  f => f.DateId)
                .IsRequired();
            modelBuilder.Entity<TimeSeriesDao>()
                .HasKey(t => t.Id)
                .HasName("TimeSerieId_PK");
            modelBuilder.Entity<TimeSeriesDao>()
                 .HasMany(t => t.Facts)
                 .WithOne(f => f.TimeSerie)
                 .HasForeignKey(f => f.TimeSerieId)
                 .IsRequired();            
            modelBuilder.Entity<TimeSeriesDao>()
                .HasIndex(x => new { x.Name, x.StockId } )
                .IsUnique();

            modelBuilder.Entity<TimeSeriesFactDao>()
                .HasKey(f => new { f.TimeSerieId, f.DateId })
                .HasName("TimeSerieFact_PK");

            modelBuilder.Entity<StockSearchResultDao>().ToTable(nameof(StockResults), t => t.ExcludeFromMigrations());
        }
    }
}
