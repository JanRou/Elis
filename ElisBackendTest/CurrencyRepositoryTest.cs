using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Currency;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Exchange;
using Microsoft.EntityFrameworkCore;

namespace ElisBackendTest {
    public class CurrencyRepositoryTest : IDisposable {
        public CurrencyRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        public async Task GetTest() {
            // Arrange
            var filter = new FilterCurrency { Code = "%k" }; // get all currencies starting with k
            var dut = new CurrencyRepository(Db);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var currencies = result.ToList();
            Assert.NotEmpty(currencies);
            Assert.True(currencies.Count > 2);
        }

        [Fact]
        public async Task AddNDeleteTest() {
            // Arrange
            var dao = new CurrencyDao() { Name = "Test", Code = "TST" };
            var dut = new CurrencyRepository(Db);

            // Act
            var result = await dut.Add(dao);
            bool deleteResult = await dut.Delete(result.Code);

            // Assert
            Assert.NotNull(result); 
            Assert.True(result.Id > 0);
            Assert.Equal(dao.Name, result.Name);
            Assert.Equal(dao.Code, result.Code);
            Assert.True(deleteResult);
        }

        // TODO DRY
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
            Db.Dispose();
        }

    }
}