using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command
{
    public class AddStockData(StockDataIn stockData, List<TimeSerieDataIn> timeSerieDataIn) : IRequest<StockDataOut>
    {
        public StockDataIn StockData { get; private set; } = stockData;
        public List<TimeSerieDataIn> TimeSerieDataIn { get; private set; } = timeSerieDataIn;
    }

    // TODO naming
    public class AddStockDataDataHandler(
          IStockHandling stockHandling
        , IMapper mapper) : IRequestHandler<AddStockData, StockDataOut>
    {
        public async Task<StockDataOut> Handle(AddStockData request, CancellationToken cancellationToken)
        {
            List<ITimeSeriesFact> timeSerieData = mapper.Map<List<ITimeSeriesFact>>(request.TimeSerieDataIn);

            // Note: the mapper converts the string date from GraphQL to a DateTime
            var timeserie = new TimeSeries(request.StockData.TimeSerieName
                                                , request.StockData.Isin, timeSerieData);
            return await stockHandling.AddData(timeserie);

        }
    }
}
