using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command
{
    public class AddStockData(StockDataIn stockData, List<TimeSerieDataIn> timeSerieDataIn) : IRequest<StockDataOut> {
        public StockDataIn StockData { get; private set; } = stockData;
        public List<TimeSerieDataIn> TimeSerieDataIn { get; private set; } = timeSerieDataIn;
    }

   
    public class AddStockDataDataHandler(
          IStockHandling stockHandling
        , IMapper mapper) : IRequestHandler<AddStockData, StockDataOut> {
        public async Task<StockDataOut> Handle(AddStockData request, CancellationToken cancellationToken) {
            var timeSerieData = mapper.Map<List<TimeSerieData>>(request.TimeSerieDataIn);
            var timeserie = new TimeSerie(request.StockData.TimeSerieName
                                                , request.StockData.Isin, timeSerieData);
            return await stockHandling.AddData(timeserie);
        }
    }
}
