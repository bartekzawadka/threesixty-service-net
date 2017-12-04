using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Threesixty.Common.Contracts;

namespace ThreesixtyService.Exception
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                await HandleException(context, ex, ex.StatusCode);
            }
            catch (System.Exception ex)
            {
                await HandleException(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleException(HttpContext context, System.Exception ex, HttpStatusCode code)
        {
            var result = JsonConvert.SerializeObject(new {error = ex.Message});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;
            return context.Response.WriteAsync(result);
        }
    }
}
