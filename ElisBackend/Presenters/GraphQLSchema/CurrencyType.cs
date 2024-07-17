using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class CurrencyType : ObjectGraphType<ICurrency> {
        public CurrencyType() {
            Field(f => f.Name).Description("The name of the currency");
            Field(f => f.Code).Description("The short name of the currency");
        }
    }
}