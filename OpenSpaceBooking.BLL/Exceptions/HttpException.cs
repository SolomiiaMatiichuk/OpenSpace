using System.Net;

namespace OpenSpaceBooking.BLL.Exceptions;

public class HttpException : Exception
{
    public HttpException(HttpStatusCode code, object errors = null)
    {
        Code = code;
        Errors = errors;
    }
    public HttpStatusCode Code { get; }
    public object Errors { get; set; }
}