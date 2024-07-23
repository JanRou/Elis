using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class DeleteCurrency(string code) : IRequest<bool> {
        public string Code { get; set; } = code;
    }

    public class DeleteCurrencyDataHandler(ICurrencyHandling currencyHandling) : IRequestHandler<DeleteCurrency, bool> {
        public async Task<bool> Handle(DeleteCurrency request, CancellationToken cancellationToken) {
            return await currencyHandling.Delete(request.Code);
        }
    }
}
