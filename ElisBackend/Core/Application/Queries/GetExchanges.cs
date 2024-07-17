using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using MediatR;

namespace ElisBackend.Core.Application.Queries {
    public class GetExchanges(FilterExchange filter) : IRequest<IEnumerable<IExchange>> {
        public FilterExchange Filter { get; set; } = filter;
    }
    public class GetExchangesHandler(IExchangeHandling exchangeHandling) : IRequestHandler<GetExchanges, IEnumerable<IExchange>> {        

        public async Task<IEnumerable<IExchange>> Handle(GetExchanges request, CancellationToken cancellationToken) {
            return await exchangeHandling.Get(request.Filter);
        }
    }
}
