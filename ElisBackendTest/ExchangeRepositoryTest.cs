using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Exchange;
using Microsoft.EntityFrameworkCore;

namespace ElisBackendTest {
    public class ExchangeRepositoryTest : IDisposable {
        public ExchangeRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        public async Task GetTest() {
            // Arrange
            var filter = new FilterExchange { Name = "x%" };
            var dut = new ExchangeRepository(Db);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var exchanges = result.ToList();
            Assert.NotEmpty(exchanges);
            Assert.True(exchanges.Count > 2);
        }

        [Fact]
        public async Task AddNDeleteTest() {
            // Arrange
            var dao = new ExchangeDao() { Name="Uninque", Country="local", Url="https://localhost"};
            var dut = new ExchangeRepository(Db);

            // Act
            var result = await dut.Add(dao);
            bool deleteResult = await dut.Delete(result.Name);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(dao.Name, result.Name);
            Assert.Equal(dao.Url, result.Url);
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