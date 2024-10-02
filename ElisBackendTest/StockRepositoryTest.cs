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
        //[Theory]
        public void DateCompareTest() {
            // Tø-hø test ... 
            // Id: 592
            // DateTimeUtc: 2019-08-01 02:00:00+02

            // Arrange
            var existingDateDao = new DateDao() {
                Id = 592,
                DateTimeUtc = new DateTime(2019, 08, 01, 00, 00, 00, DateTimeKind.Utc)
            };
            var dut = new StockRepository(Db);

            // Act
            var result = dut.GetDate(existingDateDao.Id);

            // Assert
            // use same comparison as in repository
            Assert.True( existingDateDao.DateTimeUtc == result.DateTimeUtc);
        }

        [Fact]
        //[Theory]
        public async Task CrudTimeSerieFactsTest() {
            // The test performs a CRUD with two sets of timeserie facts for the same timeserie. The first
            // fact set is added, then the second fact set is updating the first. Finally is the facts
            // deleted. In between is the facts retrieved.
            // The sets of timeserie facts are set up in two timeseries, with the same name thus
            // it become the same timeserie for the stock, Novo. The second set of facts has same dates
            // as the first, so the second update the facts of the first set. The volume is updated.
            // Precondition Nove is in the database.

            // Arrange
            string isin = "DK0062498333"; // Novo
            string timeSerieName = "PriceAndVolume";
            decimal price1 = 19801117.0m;
            decimal price2 = 19801117.0m;
            decimal volume1 = 1.0m;
            decimal volume2 = 2.0m;
            DateTime start = new DateTime(1980, 11, 10, 00, 00, 00, DateTimeKind.Utc);
            DateTime end = new DateTime(1980, 11, 30, 00, 00, 00, DateTimeKind.Utc);
            var factDaos1 = new List<TimeSerieFactDao>() {

                 new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price1, Volume = volume1 }
               , new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price1, Volume = volume1 }
            };
            var factDaos2 = new List<TimeSerieFactDao>() {

                 new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price2, Volume = volume2 }
               , new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price2, Volume = volume2 }
            };
            var timeSerieDao1 = new TimeSerieDao() {
                Name = timeSerieName,
                Facts = factDaos1
            };
            var timeSerieDao2 = new TimeSerieDao() {
                Name = timeSerieName,
                Facts = factDaos2
            };

            var dut = new StockRepository(Db);

            // Act
            var addresult1 = await dut.AddOrUpdateTimeSerieAddFacts(isin, timeSerieDao1);
            var getresult1 = await dut.GetTimeSerieFacts(isin, timeSerieName, start, end);
            
            // Reset db context. EF Core circus ...
            Db = null;
            Setup();
            dut = new StockRepository(Db);

            var addresult2 = await dut.AddOrUpdateTimeSerieAddFacts(isin, timeSerieDao2);
            var getresult2 = await dut.GetTimeSerieFacts(isin, timeSerieName, start, end);

            // Assert
            Assert.True(addresult1 > 0);
            Assert.NotNull(getresult1);
            var listgetresult1 = getresult1.ToList();
            Assert.Equal( 2, listgetresult1.Count);
            Assert.Equal(price1, listgetresult1[0].Price);
            Assert.Equal(price1, listgetresult1[1].Price);
            Assert.Equal(volume1, listgetresult1[0].Volume);
            Assert.Equal(volume1, listgetresult1[1].Volume);                        

            Assert.True(addresult2 > 0);
            Assert.NotNull(getresult2);
            var listgetresult2 = getresult2.ToList();
            Assert.Equal(2, listgetresult2.Count);
            Assert.Equal(price2, listgetresult2[0].Price);
            Assert.Equal(price2, listgetresult2[1].Price);
            Assert.Equal(volume2, listgetresult2[0].Volume);
            Assert.Equal(volume2, listgetresult2[1].Volume);

            // Clean up
            var deleteResult1 = await dut.DeleteFacts(timeSerieDao1);

            Assert.True(deleteResult1);
        }

        [Fact]
        //[Theory]
        public async Task AddTimeSerieFactsForTwoStocksAndSameDatesTest() {

            HER TIL
            // Arrange
            string isin = "DK0062498333"; // Novo
            string timeSerieName = "PriceAndVolume";
            decimal price1 = 19801117.0m;
            decimal price2 = 19801117.0m;
            decimal volume1 = 1.0m;
            decimal volume2 = 2.0m;
            DateTime start = new DateTime(1980, 11, 10, 00, 00, 00, DateTimeKind.Utc);
            DateTime end = new DateTime(1980, 11, 30, 00, 00, 00, DateTimeKind.Utc);
            var factDaos1 = new List<TimeSerieFactDao>() {

                 new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price1, Volume = volume1 }
               , new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price1, Volume = volume1 }
            };
            var factDaos2 = new List<TimeSerieFactDao>() {

                 new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price2, Volume = volume2 }
               , new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price2, Volume = volume2 }
            };
            var timeSerieDao1 = new TimeSerieDao() {
                Name = timeSerieName,
                Facts = factDaos1
            };
            var timeSerieDao2 = new TimeSerieDao() {
                Name = timeSerieName,
                Facts = factDaos2
            };

            var dut = new StockRepository(Db);

            // Act
            var addresult1 = await dut.AddOrUpdateTimeSerieAddFacts(isin, timeSerieDao1);
            var getresult1 = await dut.GetTimeSerieFacts(isin, timeSerieName, start, end);

            // Reset db context. EF Core circus ...
            Db = null;
            Setup();
            dut = new StockRepository(Db);

            var addresult2 = await dut.AddOrUpdateTimeSerieAddFacts(isin, timeSerieDao2);
            var getresult2 = await dut.GetTimeSerieFacts(isin, timeSerieName, start, end);

            // Assert
            Assert.True(addresult1 > 0);
            Assert.NotNull(getresult1);
            var listgetresult1 = getresult1.ToList();
            Assert.Equal(2, listgetresult1.Count);
            Assert.Equal(price1, listgetresult1[0].Price);
            Assert.Equal(price1, listgetresult1[1].Price);
            Assert.Equal(volume1, listgetresult1[0].Volume);
            Assert.Equal(volume1, listgetresult1[1].Volume);

            Assert.True(addresult2 > 0);
            Assert.NotNull(getresult2);
            var listgetresult2 = getresult2.ToList();
            Assert.Equal(2, listgetresult2.Count);
            Assert.Equal(price2, listgetresult2[0].Price);
            Assert.Equal(price2, listgetresult2[1].Price);
            Assert.Equal(volume2, listgetresult2[0].Volume);
            Assert.Equal(volume2, listgetresult2[1].Volume);

            // Clean up
            var deleteResult1 = await dut.DeleteFacts(timeSerieDao1);

            Assert.True(deleteResult1);
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