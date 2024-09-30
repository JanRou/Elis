using AutoMapper;
using ElisBackend;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using Moq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackendTest {
    public class AutoMapperTest {
        public AutoMapperTest()
        {
            Setup();
        }
        private IMapper _mapper;
        private void Setup() {

            _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        }

        [Fact]
        //[Theory]
        public void AutoMapperConfigurationTest() {
            // Arrange
            var config = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            // Assert
            config.AssertConfigurationIsValid();
            }
        
        [Fact]
        public void StockDaoToStockTest() {
            // Arrange
            var dao = new StockDao() {
                Name = "Dummy A/S",
                Isin = "DK0099999999",
                ExchangeId = 1,
                CurrencyId = 1
              , Exchange = new ExchangeDao() { Name="Euronext", Country="France", Url="localhost"}
              , Currency = new CurrencyDao() { Name="Norskre kroner", Code="NKK"}
            };
            // Act
            var result = _mapper.Map<Stock>(dao);

            // Assert
            Assert.Equal(dao.Name, result.Name);
            Assert.Equal(dao.Isin, result.Isin);
            Assert.Equal(dao.Exchange.Name, result.Exchange.Name);
            Assert.Equal(dao.Currency.Code, result.Currency.Code);
        }

        [Fact]
        public void StockToStockDaoTest() {
            // Arrange
            var stock = new Stock( "Dummy AB", "DK0099999999", "CSE123456", new Exchange( "Børs", "DK", "http://localhost") 
                , new Currency("Norske kroner", "NKK") );

            // Act
            var result = _mapper.Map<StockDao>(stock);

            // Assert
            Assert.Equal(stock.Name, result.Name);
            Assert.Equal(stock.Isin, result.Isin);
            Assert.Equal(stock.Exchange.Name, result.Exchange.Name);
            Assert.Equal(stock.Currency.Code, result.Currency.Code);
        }

        [Fact]
        //[Theory, AutoData]
        public void TimeSerieDataInToTimeSerieDataArbitraryDateTimeTest() {
            // Arrange
            var timeSerieDataIn = new TimeSerieDataIn("2024-07-24T14:48:00.000Z", 100.0m, 1.0m);

            // Act
            var result = _mapper.Map<TimeSerieData>(timeSerieDataIn);
            var resultReverse = _mapper.Map<TimeSerieDataIn>(result);

            // Assert
            Assert.Equal( timeSerieDataIn.Price, result.Price);
            Assert.Equal( timeSerieDataIn.Volume, result.Volume);
            Assert.Equal( timeSerieDataIn.Date, resultReverse.Date);
        }

        [Fact]
        //[Theory, AutoData]
        public void TimeSerieDataInToTimeSerieDataDateOnlyTest() {
            // Arrange
            var timeSerieDataIn = new TimeSerieDataIn("2024-07-24T00:00:00.000Z", 100.1m, 1.1m);

            // Act
            var result = _mapper.Map<TimeSerieData>(timeSerieDataIn);
            var resultReverse = _mapper.Map<TimeSerieDataIn>(result);

            // Assert
            Assert.Equal(timeSerieDataIn.Price, result.Price);
            Assert.Equal(timeSerieDataIn.Volume, result.Volume);
            Assert.Equal(timeSerieDataIn.Date, resultReverse.Date);
        }

        [Fact]
        //[Theory, AutoData]
        public void TimeSerieToTimeSerieDaoTest() {
            // Arrange
            var timeSerieData = new List<TimeSerieData>() {
                new TimeSerieData( new DateTime(2024, 07, 26, 00,00,00, DateTimeKind.Utc), 100.0m, 1.0m)
              , new TimeSerieData( new DateTime(2024, 07, 25, 00,00,00, DateTimeKind.Utc), 99.0m, 1.0m)
            };
            var timeSerie = new TimeSerie( "PricesAndVolumes", "123456", timeSerieData );

            // Act
            var result = _mapper.Map<TimeSerieDao>(timeSerie);

            // Assert
            Assert.Equal(timeSerie.Name, result.Name );
            Assert.NotNull(result.Facts);
            var facts = result.Facts.ToList();
            Assert.Equal(2, facts.Count);
            Assert.Equal(100.0m, facts[0].Price);
            Assert.Equal(1.0m, facts[0].Volume);
        }

        //[Fact]
        ////[Theory, AutoData]
        //public void Test() {
        //    // Arrange

        //    // Act

        //    // Assert

        //}        
    }
}