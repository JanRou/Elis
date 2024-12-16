using AutoMapper;
using ElisBackend;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.TimeSeries;
using Moq;

namespace ElisBackendTest
{
    public class TimeSeriesHandlingTest
    {
        public TimeSeriesHandlingTest()
        {
            Setup();
        }
        private IMapper _mapper;
        private void Setup()
        {

            _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        }


        [Fact]
        //[Theory]
        public async Task GetTimeSeriesTest()
        {
            // Arrange
            var timeSeriesRepository = new Mock<ITimeSeriesRepository>();
            var filter = new FilterTimeSerieFacts() { 
                Isin = "123456789", 
                TimeSeriesName = "PricesAndVolumes", 
                From = "1980-11-10T00:00:00.00000Z",
                To = "1980-11-12T00:00:00.00000Z"
            };
            var timeSeriesFactDaos = CreateTimeSeriesFacts();
            timeSeriesRepository.Setup(r => r.GetTimeSeriesFacts(filter.Isin, filter.TimeSeriesName, It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()))
                .ReturnsAsync(timeSeriesFactDaos);

            var dut = new TimeSeriesHandling(timeSeriesRepository.Object, _mapper);

            // Act
            var result = await dut.GetTimeSeries(filter);

            // Assert
            timeSeriesRepository.Verify();
            Assert.NotNull(result);
            Assert.Equal(timeSeriesFactDaos.Count, result.TimeSeriesData.Count);
            Assert.Equal(filter.Isin, result.Isin);
            Assert.Equal(filter.TimeSeriesName, result.Name);
            var timeSeriesData = result.TimeSeriesData.ToList();
            Assert.Equal(timeSeriesFactDaos[0].Price, timeSeriesData[0].Price);
            Assert.Equal(timeSeriesFactDaos[0].Volume, timeSeriesData[0].Volume);
            Assert.True(timeSeriesFactDaos[0].Date.DateTimeUtc.CompareTo(timeSeriesData[0].Date) == 0);
        }

        private List<TimeSeriesFactDao> CreateTimeSeriesFacts()
        {
            return new List<TimeSeriesFactDao> {
                new TimeSeriesFactDao() {
                    Date = new DateDao() { DateTimeUtc = new DateTime(1980, 11, 10, 0, 0,0, DateTimeKind.Utc ) },
                    Price = 100m,
                    Volume = 10m },
                new TimeSeriesFactDao() {
                    Date = new DateDao() { DateTimeUtc = new DateTime(1980, 11, 11, 0, 0,0, DateTimeKind.Utc ) },
                    Price = 101m,
                    Volume = 10m },
                new TimeSeriesFactDao() {
                    Date = new DateDao() { DateTimeUtc = new DateTime(1980, 11, 12, 0, 0,0, DateTimeKind.Utc ) },
                    Price = 102m,
                    Volume = 10m },
            };
        }
    }

}