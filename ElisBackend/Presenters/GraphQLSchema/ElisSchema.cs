using ElisBackend.Presenters.Dtos;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema {

    // Check schema with playground http://localhost:<port>/
    public class ElisSchema : Schema {
        public ElisSchema() : base() {
            Query = new StocksQuery();
            Mutation = new StockMutation();
            //Mutation = resolver.GetRequiredService<StockMutation>();
            //Subscription = resolver.GetRequiredService<StockSubscription>();
        }
    }
}
