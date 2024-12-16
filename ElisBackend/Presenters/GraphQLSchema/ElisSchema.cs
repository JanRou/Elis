using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema {

    // Check schema with playground http://localhost:<port>/
    public class ElisSchema : Schema {
        public ElisSchema() : base() {
            Query = new ElisQuery();
            Mutation = new ElisMutation();
            //Subscription = new ElisSubscriptions();
        }
    }
}
