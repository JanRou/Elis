using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Currency;
using Microsoft.EntityFrameworkCore;

namespace ElisBackendTest {
    public class CurrencyRepositoryTest : IDisposable {
        public CurrencyRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        public async Task GetTest() {
            // Arrange
            var filter = new FilterCurrency { Code = "%k" }; // get all with kroner
            var dut = new CurrencyRepository(Db);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var currencies = result.ToList();
            Assert.NotEmpty(currencies);
            Assert.True(currencies.Count > 2);
        }


        private static readonly object _lock = new();
        public void Setup() {
            lock (_lock) {
                if (Db == null) {
                    Db = new ElisContext(
                            new DbContextOptionsBuilder<ElisContext>()
                                .UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=31ishoj14!;Database=Elis")
                                .Options);
                }
            }
        }

        public void Teardown() {

        }

    }
}