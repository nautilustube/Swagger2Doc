using System.Net;

namespace Swagger2Doc.Models.DTO
{
    public interface IStatusCodeResp
    {
        HttpStatusCode ResponseStatusCode { get; set; }
    }
}
