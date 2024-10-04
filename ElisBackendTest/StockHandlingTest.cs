using AutoMapper;
using AutoMapper.Configuration.Annotations;
using ElisBackend;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Gateways.Repositories.TimeSeries;
using Moq;

namespace ElisBackendTest {

    public class StockHandlingTest {
        public StockHandlingTest()
        {
            Setup();
        }

        private IMapper _mapper;
        private void Setup() {

            _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        }

        [Fact]
        public async Task GetTest() {
            // Arrange
            var stockRepositoryMock = new Mock<IStockRepository>();
            var timeSeriesRepositoryMock = new Mock<ITimeSeriesRepository>();
            var filter = new FilterStock() { Name= "Test" };
            var daos = new List<StockDao>() { new StockDao() { Id=1, Name="Test", CurrencyId = 1, ExchangeId=1 } };
            stockRepositoryMock.Setup(r => r.Get(filter)).ReturnsAsync(daos);

            var dut = new StockHandling(stockRepositoryMock.Object, timeSeriesRepositoryMock.Object, _mapper);

            // Act
            var result = await dut.Get(filter);

            // Assert
            stockRepositoryMock.Verify();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var resultList = result.ToList();
            Assert.Equal("Test", resultList[0].Name);
        }

        [Fact]
        public async Task AddTest() {
            // Arrange
            var stockRepositoryMock = new Mock<IStockRepository>();
            var timeSeriesRepositoryMock = new Mock<ITimeSeriesRepository>();
            var dao = new StockDao() { Id=1, Name = "Test", Isin = "123456789", CurrencyId = 1, ExchangeId = 1
                                , Exchange = new ExchangeDao() { Id=1, Name = "Exchange", Country="Danmark", Url= "http:localhost" }
                                , Currency = new CurrencyDao() { Id=1, Name = "Danske kroner", Code="DKK"}
                            };
            var stock = new Stock( "Test", "123456789", "CSE123456"
                , new Exchange( "Exchange", null, null)
                , new Currency( null, "DKK") );
            stockRepositoryMock.Setup(r => r.Add(It.IsAny<StockDao>())).ReturnsAsync(dao);

            var dut = new StockHandling(stockRepositoryMock.Object, timeSeriesRepositoryMock.Object, _mapper);

            // Act
            var result = await dut.Add(stock);

            // Assert
            stockRepositoryMock.Verify();
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal("123456789", result.Isin);
            Assert.Equal("Exchange", result.Exchange.Name);
            Assert.Equal("DKK", result.Currency.Code);
        }

        [Fact]
        //[Theory]
        public async Task AddDataOkTest() {
            // Arrange
            int expectedCount = 1;
            string timeSerieName = "PriceAndVolume";
            string isin = "123456";

            var stockRepositoryMock = new Mock<IStockRepository>();
            var timeSeriesRepositoryMock = new Mock<ITimeSeriesRepository>();

            var timeSeriesDao = new TimeSeriesDao() { Id = 9999, Name = "PriceAndVolume", StockId = 9999 };
            timeSeriesRepositoryMock.Setup(t => t.GetTimeSeries(isin, timeSerieName)).ReturnsAsync(timeSeriesDao);
            timeSeriesRepositoryMock.Setup(t => t.AddOrUpdateTimeSeriesFacts(
                isin, It.IsAny<IEnumerable<TimeSeriesFactDao>>())).ReturnsAsync(expectedCount);

            var timeSerieData = new List<TimeSerieData>() { new TimeSerieData( DateTime.UtcNow, 100.0m, 1000.0m),};
            var timeSerie = new TimeSeries( timeSerieName, isin, timeSerieData);

            var dut = new StockHandling(stockRepositoryMock.Object, timeSeriesRepositoryMock.Object, _mapper);

            // Act
            StockDataOut result = await dut.AddData(timeSerie);

            // Assert
            stockRepositoryMock.Verify();
            Assert.NotNull(result);
            Assert.Equal(timeSerieName, result.TimeSerieName);
            Assert.Equal(isin, result.Isin);
            Assert.Equal(expectedCount, result.CountTimeSerieFacts);
            Assert.Contains("Ok", result.Status);
        }
    }

}