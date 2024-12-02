using ElisBackend.Core.Application.Dtos;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StockDataResultType : ObjectGraphType<StockDataOut> {
        public StockDataResultType() {
            Description = "Result of adding timeserie data to stock";
            Field(s => s.Isin).Description("The ISIN code for the stock");
            Field(s => s.TimeSerieName).Description("Name of timeserie added or updated with data");
            Field(s => s.CountTimeSerieFacts).Description("Number of data facts added to timeserie");
            Field(s => s.Status).Description("Status message like OK or Error including error description");
        }
    }
}


