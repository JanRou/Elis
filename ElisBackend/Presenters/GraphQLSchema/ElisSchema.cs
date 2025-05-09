using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema {

    public class ElisSchema : Schema {
        public ElisSchema() : base() {
            Query = new ElisQuery();
            Mutation = new ElisMutation();
            //Subscription = new ElisSubscriptions();
        }
    }
}
