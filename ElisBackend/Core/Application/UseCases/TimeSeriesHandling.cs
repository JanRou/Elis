using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Gateways.Repositories.TimeSeries;

namespace ElisBackend.Core.Application.UseCases
{
    public interface ITimeSeriesHandling
    {
        Task<IStockTimeSeries> GetFacts(FilterTimeSerieFacts filter);
    }

    public class TimeSeriesHandling(IStockRepository stockRepository, ITimeSeriesRepository timeSeriesRepository
        , IMapper mapper) : ITimeSeriesHandling
    {
        public async Task<IStockTimeSeries> GetFacts(FilterTimeSerieFacts filter)
        {
            IStockTimeSeries result = null;
            // TODO call timeSeriesRepo to get facts
            return result;
        }
    }
}
