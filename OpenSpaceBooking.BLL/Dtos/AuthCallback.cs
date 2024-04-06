using System.Net;
using System.Security.Claims;

namespace OpenSpaceBooking.BLL.Dtos;

public class AuthCallback
{
    public HttpStatusCode StatusCode { get; set; }
    public ClaimsIdentity ClaimsIdentity { get; set; }
}