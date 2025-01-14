using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Globalization;

namespace ElisBackend
{
    // Resulting DateTime is in UTC and the input must be in ISO 8601 format for UTC date
    public class StringToDateTimeUtcConverter : ITypeConverter<string, DateTime> {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context) {
            return DateTime.Parse( source, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }
    }

    // Resulting ISO 8601 format string in UTC from a DateTime in UTC
    public class DateTimeUtcToStringConverter : ITypeConverter<DateTime, string> {
        public string Convert(DateTime source, string destination, ResolutionContext context) {
            return source.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Basic type conversionm string to date time and reverse
            CreateMap<string, DateTime>().ConvertUsing(new StringToDateTimeUtcConverter());
            CreateMap<DateTime, string>().ConvertUsing(new DateTimeUtcToStringConverter());

            CreateMap<CurrencyDao, Currency>()
                .ReverseMap()
                ;
            CreateMap<ExchangeDao, Exchange>()
                .ReverseMap()
                ;
            CreateMap<StockDao, Stock>()
                .ReverseMap()
                ;
            CreateMap<DateTime, DateDao>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember( d=>d.DateTimeUtc, o => o.MapFrom( s => s.Date))
                .ForMember(d => d.Facts, o => o.Ignore())
                .ReverseMap()
                .ConstructUsing(s => s.DateTimeUtc )
                ;
            CreateMap<TimeSerieDataIn, ITimeSeriesFact>()
                .ConstructUsing(s => new TimeSeriesFact(
                    DateTime.Parse(s.Date, null, System.Globalization.DateTimeStyles.AdjustToUniversal)
                    , s.Price, s.Volume))
                ;
            CreateMap<TimeSerieDataIn, TimeSeriesFact>()
                .ReverseMap()
                ;
            CreateMap<TimeSeriesFact, TimeSeriesFactDao>()
                .ForMember(d => d.TimeSerieId, o => o.Ignore())
                .ForMember(d => d.TimeSerie, o => o.Ignore())
                .ForMember(d => d.DateId, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date.DateTimeUtc))
                ;
            CreateMap<TimeSeriesFactDao, ITimeSeriesFact>()
                .ConstructUsing(s => new TimeSeriesFact(s.Date.DateTimeUtc, s.Price, s.Volume))
                .ReverseMap()
                ;
            CreateMap<TimeSeries, TimeSeriesDao>()
                .ForMember(d => d.Facts, o => o.MapFrom( s => s.TimeSeriesData))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.StockId, o => o.Ignore())
                .ForMember(d => d.Stock, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.TimeSeriesData, o => o.MapFrom(s => s.Facts))
                .ForMember(d => d.Isin, o => o.MapFrom(s => s.Stock.Isin))
                ;

        }
    }
}
