using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Gateways.Repositories.TimeSeries;

namespace ElisBackend.Core.Application.UseCases
{
    public interface ITimeSeriesHandling
    {
        Task<ITimeSeries> GetTimeSeries(FilterTimeSerieFacts filter);
    }

    public class TimeSeriesHandling(ITimeSeriesRepository timeSeriesRepository
        , IMapper mapper) : ITimeSeriesHandling
    {
        public async Task<ITimeSeries> GetTimeSeries(FilterTimeSerieFacts filter)
        {
            DateTime from = mapper.Map<DateTime>(filter.From); // GraphQL has to ensure proper datetime format!
            DateTime to = mapper.Map<DateTime>(filter.To);
            var timeSeriesFactDao = await timeSeriesRepository.GetTimeSeriesFacts(filter.Isin, filter.TimeSeriesName, from, to);
            var timeSeriesFact = timeSeriesFactDao.Any() ? mapper.Map<List<ITimeSeriesFact>>(timeSeriesFactDao) : null;

            return new TimeSeries(filter.TimeSeriesName, filter.Isin, timeSeriesFact);
        }

    }
}
