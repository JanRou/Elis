using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using System.Globalization;

namespace ElisBackend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurrencyDao, Currency>()
                .ReverseMap()
                ;
            CreateMap<ExchangeDao, Exchange>()
                .ReverseMap()
                ;
            CreateMap<StockDao, Stock>()
                .ReverseMap()
             ;

            // 
            // DateTime.ParseExact( s.Date
            CreateMap<TimeSerieDataIn, TimeSerieData>()
                .ForMember( d => d.Date, o => 
                    o.MapFrom( s => DateTime.Parse( s.Date, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime() ) )
                .ReverseMap()
                .ForMember(d => d.Date, o =>
                    o.MapFrom(s => s.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffz", CultureInfo.InvariantCulture)))
                    //o.MapFrom(s => s.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)))
             ;
        }
    }
}
