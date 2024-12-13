using ElisBackend.Presenters.GraphQLSchema.Stock;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class ElisSubscriptions : ObjectGraphType
    {
        public ElisSubscriptions()
        {
            Description = "Subscribe to stock data changes";
            Field<StockSubscriptionType>("stocks").Resolve(ctx => new { });
        }
    }

    public class StockSubscriptionType : ObjectGraphType
    {
        public StockSubscriptionType()
        {
            //Field<StockDataSubscriptionResultType, Message>("subscribeToDataForStock")
            //    .Argument<NonNullGraphType<StringGraphType>>("isin")
            //    .ResolveStream(SubscribeByIsin);

        }

        //private IObservable<Message> SubscribeByIsin(IResolveFieldContext context) {
        //    string id = context.GetArgument<string>("isin");

        //    //var messages = _chat.Messages();

        //    return messages.Where(message => message.From.Id == id);
        //}


    }

    public class StockDataSubscriptionResultType : ObjectGraphType
    {
        public StockDataSubscriptionResultType()
        {
            Field<StockType>("stock").Resolve(ctx => new { });
            //Field<StockTimeSeriesType>.Resolve(ctx => new { });
            //Field<StockTimeSerieFactsType>.Resolve(ctx => new { });

        }
    }

    //public class StockDataType : ObjectGraphType<IStock> {
}
