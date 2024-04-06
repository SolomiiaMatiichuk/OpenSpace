using System.Net;

namespace OpenSpaceBooking.BLL.Dtos;

public class OpenSpaceCallback
{
    public HttpStatusCode StatusCode { get; set; }
    public string Error { get; set; }
}