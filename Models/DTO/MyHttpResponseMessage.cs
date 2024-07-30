using System.Net;

namespace Swagger2Doc.Models.DTO
{
    public class MyHttpResponseMessage
    {
        public HttpResponseMessage ResponseRawData { get; set; }

        public DateTime? StartTime { get; set; }
        public long? DurationMilliseconds { get; set; }
        public object? ResponseData { get; set; }
    }
}
