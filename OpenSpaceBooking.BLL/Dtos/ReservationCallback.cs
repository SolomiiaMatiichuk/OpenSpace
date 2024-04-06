using System.Net;

namespace OpenSpaceBooking.BLL.Dtos;

public class ReservationCallback
{
    public HttpStatusCode StatusCode { get; set; }
    public string Error { get; set; }
}