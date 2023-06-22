using System.Net;
using System.Net.Mime;
using CertificatesWebApp.Exceptions;

namespace CertificatesWebApp.Infrastructure
{
    internal static class Utils
    {
        internal static HttpStatusCode ExceptionToStatusCode(this Exception exception)
            => exception switch
            {
                InvalidInputException => HttpStatusCode.BadRequest,
                ResourceNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

        internal static async Task WriteJsonToHttpResponseAsync<TResponse>(HttpResponse httpResponse, HttpStatusCode statusCode, TResponse response)
        {
            httpResponse.ContentType = MediaTypeNames.Application.Json;
            httpResponse.StatusCode = (int)statusCode;
            await httpResponse.WriteAsJsonAsync(response);
        }
    }
}
