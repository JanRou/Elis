using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class ExchangeType : ObjectGraphType<IExchange> {
        public ExchangeType() {
            Field(f => f.Name).Description("The name of the exchange");
            Field(f => f.Country).Description("The country of the exchange");
            Field(f => f.Url).Description("The url to connect to the exchange");
        }
    }
}


