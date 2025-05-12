using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace ElisBackend.Presenters.GraphQLSchema
{
    [Route("graphql/[controller]")]
    [ApiController]
    public class SdlController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            ElisSchema schema = new ElisSchema();


            return new FileStreamResult(
                    CreateStreamForString(schema.Print())
                ,    "application/graphql") 
            {
                FileDownloadName = "schema.graphql"
            };
        }

        private Stream CreateStreamForString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
