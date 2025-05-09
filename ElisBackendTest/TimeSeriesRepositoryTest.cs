using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Gateways.Repositories.TimeSeries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElisBackendTest {
    public class TimeSeriesRepositoryTest : IDisposable {
        public TimeSeriesRepositoryTest() { Setup(); }

        public void Dispose() { Teardown(); }

        public ElisContext Db { get; set; }

        [Fact]
        //[Theory]
        public async Task GetTimeSeriesOkTest() {
            // Arrange
            string isin = "DK0062498333"; // Novo
            string timeSerieName = "PriceAndVolume";

            var dut = new TimeSeriesRepository(Db);

            // Act
            var result = await dut.GetTimeSeries( isin, timeSerieName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.False(string.IsNullOrEmpty(result.Name));
        }

        [Fact]
        //[Theory]
        public async Task GetTimeSeriesFailsTest() {
            // Arrange
            string isin = "DK00666666666"; // Doesn't exists
            string timeSerieName = "PriceAndVolume";

            var dut = new TimeSeriesRepository(Db);

            // Act
            var result = await dut.GetTimeSeries(isin, timeSerieName);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        //[Theory]
        public async Task AddTimeSeriesFactsOkTest() {
            // Arrange
            string isin = "DK0062498333"; // Novo
            string timeSerieName = "PriceAndVolume";
            decimal price1 = 19801117.0m;
            decimal price2 = 19801118.0m;
            decimal volume1 = 1.0m;
            decimal volume2 = 2.0m;
            DateTime start = new DateTime(1980, 11, 10, 00, 00, 00, DateTimeKind.Utc);
            DateTime end = new DateTime(1980, 11, 30, 00, 00, 00, DateTimeKind.Utc);

            var dut = new TimeSeriesRepository(Db);
            var timeSeries = await dut.GetTimeSeries(isin, timeSerieName);
            var facts = new List<TimeSeriesFactDao>() {
                     new TimeSeriesFactDao() {
                        TimeSerieId = timeSeries.Id
                      , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                      , Price = price1, Volume = volume1 }
                   , new TimeSeriesFactDao() {
                        TimeSerieId = timeSeries.Id
                      , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                      , Price= price2, Volume = volume2 }
                };

            // Act
            int addresult = await dut.AddOrUpdateTimeSeriesFacts(isin, facts);
            // Reset db context. EF Core circus ...
            Teardown();
            Setup();
            dut = new TimeSeriesRepository(Db);
            var getresult = await dut.GetTimeSeriesFacts(isin, timeSerieName, start, end);

            // Assert
            Assert.True(addresult > 0);
            Assert.NotNull(getresult);
            var listgetresult = getresult.ToList();
            Assert.Equal(2, listgetresult.Count);
            Assert.Equal(price1, listgetresult[0].Price);
            Assert.Equal(price2, listgetresult[1].Price);
            Assert.Equal(volume1, listgetresult[0].Volume);
            Assert.Equal(volume2, listgetresult[1].Volume);

            // Clean up
            var deleteResult1 = await dut.DeleteFacts(listgetresult);

        }
        [Fact]
        //[Theory]
        public async Task UpdateTimeSerieFactsOkTest() {
            // Arrange
            string isin = "DK0062498333"; // Novo
            string timeSerieName = "PriceAndVolume";
            decimal price1 = 19801117.0m;
            decimal price2 = 19801118.0m;
            decimal volume1 = 1.0m;
            decimal volume2 = 2.0m;
            DateTime start = new DateTime(1980, 11, 10, 00, 00, 00, DateTimeKind.Utc);
            DateTime end = new DateTime(1980, 11, 30, 00, 00, 00, DateTimeKind.Utc);

            var dut = new TimeSeriesRepository(Db);
            var timeSeries = await dut.GetTimeSeries(isin, timeSerieName);
            var factDaos1 = new List<TimeSeriesFactDao>() {
                 new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price1, Volume = volume1 }
               , new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price1, Volume = volume1 }
            };
            var factDaos2 = new List<TimeSeriesFactDao>() {

                 new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price2, Volume = volume2 }
               , new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price2, Volume = volume2 }
            };

            // Act
            var addresult1 = await dut.AddOrUpdateTimeSeriesFacts(isin, factDaos1);
            // Reset db context. EF Core circus ...
            Teardown();
            Setup();
            dut = new TimeSeriesRepository(Db);
            var getresult1 = await dut.GetTimeSeriesFacts(isin, timeSerieName, start, end);

            // Reset db context. EF Core circus ...
            Teardown();
            Setup();
            dut = new TimeSeriesRepository(Db);

            // Update facts
            var addresult2 = await dut.AddOrUpdateTimeSeriesFacts(isin, factDaos2);

            // Reset db context. EF Core circus ...
            Teardown();
            Setup();
            dut = new TimeSeriesRepository(Db);
            var getresult2 = await dut.GetTimeSeriesFacts(isin, timeSerieName, start, end);

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
            var deleteResult1 = await dut.DeleteFacts(listgetresult2);

            Assert.True(deleteResult1);
        }
        [Fact]
        //[Theory]
        public async Task AddTimeSeriesFactsForTwoStocksAndSameDatesTest() {
            
            // TODO miss check duplets in Dates table

            // Arrange
            string isin1 = "DK0062498333"; // Novo
            string isin2 = "DK0010219153"; // Rockwoool
            string timeSerieName = "PriceAndVolume";
            decimal price1 = 19801117.0m;
            decimal price2 = 19801118.0m;
            decimal volume1 = 1.0m;
            decimal volume2 = 2.0m;
            DateTime start = new DateTime(1980, 11, 10, 00, 00, 00, DateTimeKind.Utc);
            DateTime end = new DateTime(1980, 11, 30, 00, 00, 00, DateTimeKind.Utc);
            // get timeSeries descriptions
            var dut = new TimeSeriesRepository(Db);
            var timeSeries1 = await dut.GetTimeSeries(isin1, timeSerieName);
            var timeSeries2 = await dut.GetTimeSeries(isin2, timeSerieName);

            var factDaos1 = new List<TimeSeriesFactDao>() {
                new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries1.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price1, Volume = volume1 }
               , new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries1.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price2, Volume = volume2 }
            };
            var factDaos2 = new List<TimeSeriesFactDao>() {
                 new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries2.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 17, 00,00,00, DateTimeKind.Utc) }
                  , Price = price1, Volume = volume1 }
               , new TimeSeriesFactDao() {
                    TimeSerieId = timeSeries2.Id
                  , Date = new DateDao() { DateTimeUtc=new DateTime(1980, 11, 18, 00,00,00, DateTimeKind.Utc) }
                  , Price= price2, Volume = volume2 }
            };

            // Act
            var addresult1 = await dut.AddOrUpdateTimeSeriesFacts(isin1, factDaos1);
            var addresult2 = await dut.AddOrUpdateTimeSeriesFacts(isin2, factDaos2);

            // Reset db context. EF Core circus ...
            Db = null;
            Setup();
            dut = new TimeSeriesRepository(Db);

            var getresult1 = await dut.GetTimeSeriesFacts(isin1, timeSerieName, start, end);
            var getresult2 = await dut.GetTimeSeriesFacts(isin2, timeSerieName, start, end);

            // Assert
            Assert.True(addresult1 > 0);
            Assert.NotNull(getresult1);
            var listgetresult1 = getresult1.ToList();
            Assert.Equal(2, listgetresult1.Count);
            Assert.Equal(price1, listgetresult1[0].Price);
            Assert.Equal(price2, listgetresult1[1].Price);
            Assert.Equal(volume1, listgetresult1[0].Volume);
            Assert.Equal(volume2, listgetresult1[1].Volume);

            Assert.True(addresult2 > 0);
            Assert.NotNull(getresult2);
            var listgetresult2 = getresult2.ToList();
            Assert.Equal(2, listgetresult2.Count);
            Assert.Equal(price1, listgetresult2[0].Price);
            Assert.Equal(price2, listgetresult2[1].Price);
            Assert.Equal(volume1, listgetresult2[0].Volume);
            Assert.Equal(volume2, listgetresult2[1].Volume);

            // Clean up
            var deleteResult1 = await dut.DeleteFacts(listgetresult1);
            var deleteResult2 = await dut.DeleteFacts(listgetresult2);

            Assert.True(deleteResult1);
            Assert.True(deleteResult2);
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