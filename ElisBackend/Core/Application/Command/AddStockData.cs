using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command
{
    public class AddStockData(StockDataIn stockData, List<TimeSerieDataIn> timeSerieDataIn) : IRequest<IStock> {
        public StockDataIn StockData { get; private set; } = stockData;
        public List<TimeSerieDataIn> TimeSerieDataIn { get; private set; } = timeSerieDataIn;
    }

    // TODO
    public class AddStockDataDataHandler(IStockHandling stockHandling, IMapper mapper) : IRequestHandler<AddStockData, IStock> {
        public async Task<IStock> Handle(AddStockData request, CancellationToken cancellationToken) {
            var stock = new Stock( "", request.StockData.Isin, null, null);
            var timeSerieData = mapper.Map<List<TimeSerieData>>(request.TimeSerieDataIn);
            var timeserie = new TimeSerie(request.StockData.TimeSerieName, request.StockData.Isin, timeSerieData);
            return await stockHandling.AddData(stock,timeserie);
        }
    }
}
