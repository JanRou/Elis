using ElisBackend.Core.Application.Dtos;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StockInputType : InputObjectGraphType<StockIn> {
        public StockInputType() {
            Name = "StockInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("isin");
            Field<NonNullGraphType<StringGraphType>>("instrumentcode");
            Field<NonNullGraphType<StringGraphType>>("exchangename");
            Field<NonNullGraphType<StringGraphType>>("currencycode");
        }
    }
}


