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
            CreateMap<TimeSerieDataIn, TimeSerieData>()
                .ReverseMap()
                ;
            CreateMap<TimeSerieData, TimeSerieFactDao>()
                .ReverseMap()
                ;
            CreateMap<TimeSerie, TimeSerieDao>()
                .ForMember( d => d.Facts, o => o.MapFrom( s => s.TimeSerieData))
                .ReverseMap()
                .ForMember(d => d.TimeSerieData, o => o.MapFrom(s => s.Facts))
                ;

        }
    }
}
