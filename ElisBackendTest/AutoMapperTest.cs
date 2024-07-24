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
            var stock = new Stock( "Dummy AB", "DK0099999999", new Exchange( "Børs", "DK", "http://localhost") 
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
        public void TimeSerieDataInToTimeSerieDataTest() {
            // Arrange
            var timeSerieDataIn = new TimeSerieDataIn("2024-07-24T14:48:00.000z", 100.0m, 1.0m);

            // Act
            var result = _mapper.Map<TimeSerieData>(timeSerieDataIn);
            var resultReverse = _mapper.Map<TimeSerieDataIn>(result);

            // Assert
            Assert.Equal( timeSerieDataIn.Price, result.Price);
            Assert.Equal( timeSerieDataIn.Volume, result.Volume);
            Assert.Equal( timeSerieDataIn.Date, resultReverse.Date);
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