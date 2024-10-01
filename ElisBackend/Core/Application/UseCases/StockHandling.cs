using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Core.Application.UseCases {

    public interface IStockHandling
    {
        Task<IEnumerable<IStock>> Get(FilterStock filter);
        Task<IStock> Add(IStock stock);
        Task<bool> Delete(int id);
        Task<bool> Delete(string isin);
        Task<StockDataOut> AddData(TimeSerie timeSerie);
    }

    public class StockHandling(IStockRepository repository, IMapper mapper) : IStockHandling {
        public async Task<IEnumerable<IStock>> Get(FilterStock filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Stock>>(result);
        }

        public async Task<IStock> Add(IStock stock) {
            var stockDao = mapper.Map<StockDao>(stock);
            var addedStockDao = await repository.Add(stockDao);
            return mapper.Map<Stock>(addedStockDao);
        }

        public async Task<StockDataOut> AddData(TimeSerie timeSerie) {
            ( bool isOk, string status) = Validate(timeSerie);
            int result = 0;
            if (isOk) {
                var timeSerieDao = mapper.Map<TimeSerieDao>(timeSerie);
                result = await repository.AddOrUpdateTimeSerieAddFacts(timeSerie.Isin, timeSerieDao);
            }

            return new StockDataOut( timeSerie.Isin, timeSerie.Name, result, status);
        }

        // TODO could validate all, now it stops at first error
        private (bool,string) Validate(TimeSerie timeSerie) {            
            (bool isOk ,string status) result = ValidateData((timeSerie));
            result = result.isOk ? ValidateDates(timeSerie) : result;

            return result;
        }

        private (bool,string) ValidateDates(TimeSerie timeSerie) {
            (bool isOk, string status) = (true, "Ok");
            for (int i = 0; isOk && i + 1 < timeSerie.TimeSerieData.Count; i++) {
                // check that the i'the date is before the next date
                isOk = timeSerie.TimeSerieData[i].Date.CompareTo(timeSerie.TimeSerieData[i + 1].Date) < 0;
            }
            if (!isOk) {
                status = "Error, dates are no consecutive with latest first";
            }
            return (isOk, status);
        }

        private (bool,string) ValidateData(TimeSerie timeSerie) {
            (bool isOk, string status) = (true, "Ok");
            isOk = !(string.IsNullOrEmpty(timeSerie.Isin) || string.IsNullOrEmpty(timeSerie.Name));
            if (!isOk) {
                status = "Error, Isin or Name are missing.";
            }
            return ( isOk, status);
        }

        public async Task<bool> Delete(int id) {
            return await repository.DeleteStock(id);
        }

        public async Task<bool> Delete(string isin) {
            return await repository.DeleteStock(isin);
        }
    }
}
