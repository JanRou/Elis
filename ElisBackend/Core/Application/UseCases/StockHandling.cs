using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Gateways.Repositories.TimeSeries;

namespace ElisBackend.Core.Application.UseCases {

    public interface IStockHandling
    {
        Task<IEnumerable<IStock>> Get(FilterStock filter);
        Task<IStock> Add(IStock stock);
        Task<bool> Delete(int id);
        Task<bool> Delete(string isin);
        Task<StockDataOut> AddData(TimeSeries timeSerie);
    }

    public class StockHandling(IStockRepository stockRepository, ITimeSeriesRepository timeSeriesRepository
        , IMapper mapper) : IStockHandling {
        public async Task<IEnumerable<IStock>> Get(FilterStock filter) {
            var result = await stockRepository.Get(filter);
            return mapper.Map<IEnumerable<Stock>>(result);
        }

        public async Task<IStock> Add(IStock stock) {
            var stockDao = mapper.Map<StockDao>(stock);
            var addedStockDao = await stockRepository.Add(stockDao);
            return mapper.Map<Stock>(addedStockDao);
        }

        public async Task<StockDataOut> AddData(TimeSeries timeSeries) {
            ( bool isOk, string status) = Validate(timeSeries);
            int result = 0;
            var timeSeriesDao = mapper.Map<TimeSeriesDao>(timeSeries); // Note: mapper doesn't map timeSeriesId
            int existingTimeSeriesId = await GetOrAddTimeSeries(timeSeries.Isin, timeSeries.Name);

            if (existingTimeSeriesId > 0) {
                // Set timeSeriesId on all facts
                timeSeriesDao.Facts = timeSeriesDao.Facts.Where(t => t.TimeSerieId == 0)
                    .Select(t => new TimeSeriesFactDao() {
                          Date = t.Date
                        , TimeSerieId = existingTimeSeriesId
                        , Price = t.Price
                        , Volume = t.Volume
                    });
                result = await timeSeriesRepository.AddOrUpdateTimeSeriesFacts(timeSeries.Isin, timeSeriesDao.Facts);
            }

            return new StockDataOut( timeSeries.Isin, timeSeries.Name, result, status);
        }

        private async Task<int> GetOrAddTimeSeries( string isin, string timeSeriesName) {
            int result = 0;
            int stockId = 0;

            var existingTimeSeriesDao = await timeSeriesRepository.GetTimeSeries(isin, timeSeriesName);
            if (existingTimeSeriesDao != null) {
                result = existingTimeSeriesDao.Id;
            }
            else {
                // Add new time series, so get the stock id
                var stocks = await stockRepository.Get(new FilterStock() { Isin = isin });
                stockId = stocks != null ? stocks.ToList()[0].Id : 0;
                if (stockId != 0) {
                    var timeSerisDao = new TimeSeriesDao() { StockId = stockId, Name = timeSeriesName };
                    result = await timeSeriesRepository.AddTimeSeries(timeSerisDao);
                }
            }            

            return result;
        }

        // TODO could validate all, now it stops at first error
        private (bool,string) Validate(TimeSeries timeSeries) {            
            (bool isOk ,string status) result = ValidateData((timeSeries));
            result = result.isOk ? ValidateDates(timeSeries) : result;

            return result;
        }

        private (bool,string) ValidateDates(TimeSeries timeSeries) {
            (bool isOk, string status) = (true, "Ok");
            for (int i = 0; isOk && i + 1 < timeSeries.TimeSerieData.Count; i++) {
                // check that the i'the date is before the next date
                isOk = timeSeries.TimeSerieData[i].Date.CompareTo(timeSeries.TimeSerieData[i + 1].Date) < 0;
            }
            if (!isOk) {
                status = "Error, dates are no consecutive with latest first";
            }
            return (isOk, status);
        }

        private (bool,string) ValidateData(TimeSeries timeSerie) {
            (bool isOk, string status) = (true, "Ok");
            isOk = !(string.IsNullOrEmpty(timeSerie.Isin) || string.IsNullOrEmpty(timeSerie.Name));
            if (!isOk) {
                status = "Error, Isin or Name are missing.";
            }
            return ( isOk, status);
        }

        public async Task<bool> Delete(int id) {
            return await stockRepository.DeleteStock(id);
        }

        public async Task<bool> Delete(string isin) {
            return await stockRepository.DeleteStock(isin);
        }
    }
}
