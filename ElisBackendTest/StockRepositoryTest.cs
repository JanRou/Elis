using ElisBackend.Application.UseCases;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElisBackendTest {
    public class StockRepositoryTest : IDisposable {
        public StockRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        public async Task GetTest() {
            // Arrange
            var filter = new StockFilter { Name = "Novo%" };
            var dut = new StockRepository(Db);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var stocks = result.ToList();
            Assert.NotEmpty(stocks);
            Assert.Equal(2, stocks.Count);
        }

        [Fact]
        public async Task AddAndDeleteTest() {
            // Arrange
            var stock = new StockDao("Dummy A/S", "DK0099999999", 1, 1);
            var dut = new StockRepository(Db);

            // Act add
            var addResult = await dut.Add(stock);

            // Assert add
            Assert.NotNull(addResult);
            Assert.True(addResult.Id != 0);

            // Act delete
            var deleteResult = await dut.Delete(stock.Id);

            // Assert delete
            Assert.True(deleteResult);
        }

        //[Fact]
        ////[Theory]
        //public async Task Test() {
        //    // Arrange

        //    // Act

        //    // Assert

        //}       

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
