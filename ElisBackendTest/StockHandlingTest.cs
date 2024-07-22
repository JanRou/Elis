using AutoMapper;
using ElisBackend;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
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
            var repositoryMock = new Mock<IStockRepository>();            
            var filter = new FilterStock() { Name= "Test" };
            var daos = new List<StockDao>() { new StockDao() { Id=1, Name="Test", CurrencyId = 1, ExchangeId=1 } };
            repositoryMock.Setup(r => r.Get(filter)).ReturnsAsync(daos);

            var dut = new StockHandling(repositoryMock.Object, _mapper);

            // Act
            var result = await dut.Get(filter);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var resultList = result.ToList();
            Assert.Equal("Test", resultList[0].Name);
        }

        [Fact]
        public async Task AddTest() {
            // Arrange
            var repositoryMock = new Mock<IStockRepository>();
            var dao = new StockDao() { Id=1, Name = "Test", Isin = "123456789", CurrencyId = 1, ExchangeId = 1
                                , Exchange = new ExchangeDao() { Id=1, Name = "Exchange", Country="Danmark", Url= "http:localhost" }
                                , Currency = new CurrencyDao() { Id=1, Name = "Danske kroner", Code="DKK"}
                            };
            var stock = new Stock( "Test", "123456789"
                , new Exchange( "Exchange", null, null)
                , new Currency( null, "DKK") );
            repositoryMock.Setup(r => r.Add(It.IsAny<StockDao>())).ReturnsAsync(dao);

            var dut = new StockHandling(repositoryMock.Object, _mapper);

            // Act
            var result = await dut.Add(stock);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal("123456789", result.Isin);
            Assert.Equal("Exchange", result.Exchange.Name);
            Assert.Equal("DKK", result.Currency.Code);
        }

        //[Fact]
        ////[Theory]
        //public void Test() {
        //    // Arrange

        //    // Act

        //    // Assert

        //}

    }

}