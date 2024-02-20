using ElisBackend.Application.UseCases;
using ElisBackend.Gateways.Repositories.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElisBackendTest {
    public class StockRepositoryTest {
        [Fact]
        //[Theory]
        public async Task GetTest() {
            // Arrange
            var filter = new StockFilter { Name = "Novo" };
            var dut = new StockRepository();

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var stocks = result.ToList();
            Assert.NotEmpty(stocks);
            Assert.Equal(2, stocks.Count);
        }

        //[Fact]
        ////[Theory]
        //public async Task Test() {
        //    // Arrange

        //    // Act

        //    // Assert

        //}

    }
}
