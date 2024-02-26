using ElisBackend.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ElisBackend.Gateways.Dal {

    public class ElisContext : DbContext {

        public ElisContext(DbContextOptions<ElisContext> options) : base(options) { }

        public DbSet<StockDao> Stocks { get; set; }        
        public DbSet<ExchangeDao> Exchanges { get; set; }
        public DbSet<CurrencyDao> Currencies { get; set; }

        public DbSet<StockSearchResultDao> StockResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockDao>()
                .HasKey(x => x.Id)
                .HasName("StockId_PK");                
            ;

            modelBuilder.Entity<ExchangeDao>()
                .HasKey(x => x.Id)
                .HasName("ExchangeId_PK");
            ;

            modelBuilder.Entity<CurrencyDao>()
                .HasKey(x => x.Id)
                .HasName("CurrencyId_PK");
            ;

            modelBuilder.Entity<StockSearchResultDao>().ToTable(nameof(StockResults), t => t.ExcludeFromMigrations());
        }
    }
}
