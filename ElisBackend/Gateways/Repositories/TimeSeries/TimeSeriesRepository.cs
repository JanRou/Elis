using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElisBackend.Gateways.Repositories.TimeSeries {

    public interface ITimeSeriesRepository {

        /// <summary>
        /// Gets an existing time series description.
        /// </summary>
        /// <param name="isin">Isin of the stock</param>
        /// <param name="timeSeriesName">Name of the time series</param>
        /// <returns>The time series</returns>
        Task<TimeSeriesDao?> GetTimeSeries(string isin, string timeSeriesName);

        /// <summary>
        /// Adds a new time series description. It's not allowed to have Facts in the dao.
        /// </summary>
        /// <param name="timeSeries">The time series to add with no facts</param>
        /// <returns>Id of the added time series, otherwise 0</returns>
        Task<int> AddTimeSeries(TimeSeriesDao timeSeries);

        /// <summary>
        /// Adds or updates timeseries facts to the stock identified by isin code. The time series has to exist.
        /// The Date property of the fact has to be present. The method either adds a new date to the Dates table
        /// or replaces the Date property with the reference to an existing date in DateId property.
        /// </summary>
        /// <param name="isin">Isin code for the stock</param>
        /// <param name="timeSeries">TimeSerie to add</param>
        /// <returns>Count of facts processed</returns>
        Task<int> AddOrUpdateTimeSeriesFacts(string isin, IEnumerable<TimeSeriesFactDao> timeSeriesFacts);

        /// <summary>
        /// Gets the timeseries facts for a timeserie belonging to a stock in a givend period.
        /// </summary>
        /// <param name="isin">Isin for stock</param>
        /// <param name="timeSeriesName">Name og timeserie</param>
        /// <param name="start">Start date of the timeserie fact included</param>
        /// <param name="end">End date of the timeserie fact not included</param>
        /// <returns>Timeserie facts found</returns>
        Task<IEnumerable<TimeSeriesFactDao>> GetTimeSeriesFacts(string isin, string timeSeriesName
            , DateTime start, DateTime end);

    }

    public class TimeSeriesRepository(ElisContext db) : ITimeSeriesRepository {

        public async Task<TimeSeriesDao?> GetTimeSeries(string isin, string timeSeriesName) {
            TimeSeriesDao? result = null;
            var stock = await db.Stocks
                .Include(s => s.TimeSeries)
                .SingleOrDefaultAsync(s => s.Isin == isin);
            if (stock != null) {
                // TimeSerie exist, get id
                var timeSeries = stock.TimeSeries.Where(t => t.Name == timeSeriesName).ToList();
                result = (timeSeries.Count > 0) ? timeSeries[0] : null;
            }

            return result;
        }

        public async Task<int> AddTimeSeries(TimeSeriesDao timeSeries) {
            if (timeSeries.Facts.Count() > 0) {
                throw new InvalidDataException("TimeSeries can't have related entries for Facts.");
            }
            var exisingTimeSeries = await db.TimeSeries.SingleOrDefaultAsync(t => t.Id == timeSeries.Id);
            int result = 0;
            if (exisingTimeSeries == null) {
                db.Add(timeSeries);
                await db.SaveChangesAsync();
                result = timeSeries.Id;
            }

            return result;
        }

        public async Task<int> AddOrUpdateTimeSeriesFacts(string isin, IEnumerable<TimeSeriesFactDao> timeSeriesFacts) {
            int result = 0;
            try {
                foreach (var fact in timeSeriesFacts) {
                    var existingDate = await GetDate(fact.Date.DateTimeUtc);
                    if (existingDate != null) {
                        fact.Date = null;
                        fact.DateId = existingDate.Id;
                        db.Dates.Attach(existingDate);
                    }
                    // Get existing fact at existing date, otherwise no existing fact thus null
                    var existingFact = fact.DateId != 0 ? db.TimeSerieFacts.Find(fact.TimeSerieId, fact.DateId) : null;
                    if (existingFact != null) {
                        // Update existing with new price and volue for the date
                        existingFact.Price = fact.Price;
                        existingFact.Volume = fact.Volume;
                        db.Update(existingFact);
                    }
                    else {
                        db.Add(fact);  // Note: EF adds related Date entry that doesn't exist
                    }
                    result++;
                }

                await db.SaveChangesAsync();
            }
            catch (Exception ex) {
                // TODO use Serilog to log
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<IEnumerable<TimeSeriesFactDao>> GetTimeSeriesFacts(string isin, string timeSerieName
                , DateTime start, DateTime end) {
            List<TimeSeriesFactDao> result = null;
            var stock = db.Stocks.Where(s => s.Isin == isin).First();
            TimeSeriesDao timeSerie = null;
            if (stock != null) {
                timeSerie = db.TimeSeries.Where(t => t.StockId == stock.Id && t.Name == timeSerieName).First();
            }
            if (timeSerie != null) {
                result = await db.TimeSerieFacts.Where(f => f.TimeSerieId == timeSerie.Id
                    && f.Date.DateTimeUtc.CompareTo(start) >= 0
                    && f.Date.DateTimeUtc.CompareTo(end) < 0)
                    .Include(f => f.Date)
                    .OrderBy(f => f.Date)
                    .ToListAsync();
            }

            return result;
        }

        private async Task<DateDao?> GetDate(DateTime date) {
            return await db.Dates.SingleOrDefaultAsync(d => d.DateTimeUtc == date);
        }

        private int GetDateId(DateTime date) {
            int result = 0;
            if (db.Dates.Any(d => d.DateTimeUtc == date)) {
                var existingDate = db.Dates.Where(d => d.DateTimeUtc == date).First();
                db.Attach(existingDate);
                result = existingDate.Id;
            }

            return result;
        }

        // For unit test
        public DateDao GetDate(int id) {
            return db.Dates.Single<DateDao>(d => d.Id == id);
        }

        // Used to clean up after unit tests
        public async Task<bool> DeleteFacts(List<TimeSeriesFactDao> timeSeriesFacts) {
            foreach (var fact in timeSeriesFacts) {
                db.Remove(fact);
            }
            return 0 != await db.SaveChangesAsync();

        }
    }
}
