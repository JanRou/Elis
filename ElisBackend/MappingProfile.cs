using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;

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
        }
    }
}
