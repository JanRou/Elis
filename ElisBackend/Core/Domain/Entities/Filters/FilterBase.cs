using System.Data;

namespace ElisBackend.Core.Domain.Entities.Filters
{
    public class FilterBase
    {
        public int Take { get; set; } = 0; // 0 betyder hent alt
        public int Skip { get; set; } = 0; // default er skip intet
    }

}
