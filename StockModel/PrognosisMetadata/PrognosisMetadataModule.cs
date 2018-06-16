using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using ED.Wp3.Server.BE.PrognosisMetadata.Model;
using Json = ED.Wp3.Server.BE.PrognosisMetadata.Model.Json;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{
    public class PrognosisMetadataModule : NancyModule        
    {
        public PrognosisMetadataModule(IModel model)
            : base("/v1/metadata")
        {
            Get["/production"] = p =>
            {
                return JsonConvert.SerializeObject(model.ToJson(model.Get("MetaData/ProductionPrognosis"))
                    , new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto});
            };


            Get["/weather"] = p =>
            {
                return new JsonResponse( "Not yet implemented", new DefaultJsonSerializer());
            };

        }
    }
}
