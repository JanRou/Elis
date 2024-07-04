using AutoFixture.Xunit2;
using AutoMapper;
using ElisBackend;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
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
        
        //[Fact]
        [Theory, AutoData]
        public void StockDaoToStockTest(StockDao dao) {
            // Arrange

            // Act
            var result = _mapper.Map<Stock>(dao);

            // Assert
            Assert.Equal(dao.Name, result.Name);
            Assert.Equal(dao.Isin, result.Isin);
            Assert.Equal(dao.Exchange.Name, result.Exchange.Name);
            Assert.Equal(dao.Currency.Code, result.Currency.Code);
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