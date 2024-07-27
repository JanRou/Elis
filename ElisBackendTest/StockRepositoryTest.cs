using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
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
            var filter = new FilterStock { Name = "Novo%" };
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
        //[Theory]
        public async Task GetSortedAndPaginatedTest() {
            // Arrange
            int take = 3;
            var filter = new FilterStock() { Isin = "DK%", OrderBy = "Isin", Take = 3, Skip = 2 };
            var dut = new StockRepository(Db);

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
        }

        [Fact]
        public async Task AddAndDeleteTest() {
            // Arrange
            var stock = new StockDao() { Name = "Dummy A/S", Isin = "DK0099999999"
                , ExchangeId = 1
                , CurrencyId = 1 };
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
        public async Task DataCompareTest() {
            // Arrange
            var existingDateDao = new DateDao() {
                Id = 1,
                DateTimeUtc = new DateTime(2024, 07, 26, 00, 00, 00, DateTimeKind.Utc)
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
        public async Task AddOrUpdateTimeSerieAddFactsTest() {
            // Arrange
            var factDaos= new List<TimeSerieFactDao>() {
                new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= 19801118.0m, Volume = 1.0m }
              , new TimeSerieFactDao() {
                    Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = 19801117.0m, Volume = 1.0m }
            };
            var timeSerieDao = new TimeSerieDao() {
                Name = "PricesAndVolumes",
                Facts = factDaos
            };
            string isin = "DK0062498333"; // Novo
            var dut = new StockRepository(Db);

            // Act
            var addresult = await dut.AddOrUpdateTimeSerieAddFacts(isin, timeSerieDao);
            var deleteResult = await dut.DeleteFacts(isin, timeSerieDao);

            // Assert
            Assert.True(addresult > 0);
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
            Db.Dispose();
        }

    }

}