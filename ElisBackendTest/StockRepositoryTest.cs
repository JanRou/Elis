using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.Replication.PgOutput.Messages;
using System.ComponentModel;

namespace ElisBackendTest {

    public class StockRepositoryTest : IDisposable {
        public StockRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        public async Task AddAndDeleteTest() {
            // Arrange
            StockDao stock = CreateStock();
            var dut = new StockRepository(Db);

            // Act add
            var addResult = await dut.Add(stock);

            // Assert add
            Assert.NotNull(addResult);
            Assert.True(addResult.Id > 1);

            // Act delete
            var deleteResult = await dut.DeleteStock(stock.Isin);

            // Assert delete
            Assert.True(deleteResult);
        }



        [Fact]
        public async Task GetTest() {
            // Arrange
            var dut = new StockRepository(Db);

            // Create and add stock to db
            // First set of stocks containing filter text
            string name1 = "Ovon";
            string isin1 = "DK0062333333";
            string instrumentCode1 = "CSE12345";
            int count1 = 2;
            var isins1 = await AddStocksToSearchForInDb(dut, name1, isin1, instrumentCode1, count1);
            // Second set of stocks that don't contain filter text 
            var name2 = "Rockwool";
            var isin2 = "DK0062444444";
            var instrumentCode2 = "CSE54321";
            int count2 = 3;            
            var isins2 = await AddStocksToSearchForInDb(dut, name2, isin2, instrumentCode2, count2);
            var filter = new FilterStock { Name = name1 + "%" };

            // Reset db context after configuration of the database. EF Core circus ...
            Db = null;
            Setup();
            dut = new StockRepository(Db);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var stocks = result.ToList();
            Assert.NotEmpty(stocks);
            Assert.Equal(count1, stocks.Count);

            // Assert fields in daos returned
            Assert.Contains(name1, stocks[0].Name);
            Assert.Contains(name1, stocks[1].Name);
            Assert.Contains(isin1, stocks[0].Isin);
            Assert.Contains(isin1, stocks[1].Isin);
            Assert.Contains(instrumentCode1, stocks[0].InstrumentCode);
            Assert.Contains(instrumentCode1, stocks[1].InstrumentCode);

            // Cleanup
            await DeleteStocksAdded(dut, isins1);
            await DeleteStocksAdded(dut, isins2);

        }

        [Fact]
        //[Theory]
        public async Task GetSortedAndPaginatedTest() {
            // Arrange
            int take = 3;
            var filter = new FilterStock() { Isin = "DK%", OrderBy = "Isin", Take = 3, Skip = 2 };
            var dut = new StockRepository(Db);

            // TODO Keep simple unit tests by spinning up a populated db in Docker
            var isins = await AddStocksToSearchForInDb(dut, "stock", "DK1234560", "CSE12345", 7);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            var stocks = result.ToList();
            // Check number taken of result
            Assert.True(stocks.Count == take);
            // Check sorting
            var previous = stocks[0];
            for (int i = 1; i < take; i++) {
                Assert.True(string.Compare(previous.Isin, stocks[i].Isin) < 0);
            }

            // Cleanup
            await DeleteStocksAdded(dut, isins);
        }

        private async Task DeleteStocksAdded(IStockRepository stockRepository, List<string> isins) {
            foreach (var isin in isins) {
                await stockRepository.DeleteStock(isin);
            }
        }

        /// <summary>
        /// Add stocks to the database
        /// </summary>
        /// <param name="stockRepository">The database respository</param>
        /// <param name="nameTemplate">Template for the stock that a number is added to</param>
        /// <param name="isinTemplate">Template for isin codes that a number is added to</param>
        /// <param name="instrumentCodeTemplate">Template for instrument codes  that a number is added to</param>
        /// <param name="count">number og stocks to add</param>
        /// <returns>list of isin codes for the stocks added</returns>
        private async Task<List<string>> AddStocksToSearchForInDb(
              IStockRepository stockRepository
            , string nameTemplate = "stock"
            , string isinTemplate = "isin"
            , string instrumentCodeTemplate = "cse"
            , int count = 2) {
            var result = new List<string>();
            for (int i = 0; i < count; i++) {
                var istr = i.ToString();
                var stock = CreateStock(
                        name: nameTemplate + istr
                    , isin: isinTemplate + istr
                    , instrumentCode: instrumentCodeTemplate + istr);
                var dbResult = await stockRepository.Add(stock);
                result.Add(dbResult.Isin);
            }
            return result;
        }

        private StockDao CreateStock(
                string name = "Dummy A/S"
            ,   string isin = "DK0099999999"
            ,   string instrumentCode = "CSE123456"
            ,   int echangeId = 1
            ,   int currencyId = 1
        ) {
            return new StockDao() {
                Name = name,
                Isin = isin,
                InstrumentCode = instrumentCode,
                ExchangeId = echangeId,
                CurrencyId = currencyId
            };
        }

        // TODO DRY
        private static readonly object _lock = new();
        public void Setup() {
            lock (_lock) {
                Db = new ElisContext(
                        new DbContextOptionsBuilder<ElisContext>()
                            .UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=31ishoj14!;Database=Elis")
                            .LogTo(Console.WriteLine, LogLevel.Information)
                            .Options
                            );
            }
        }

        public void Teardown() {
            Db.Dispose();
        }

    }

}