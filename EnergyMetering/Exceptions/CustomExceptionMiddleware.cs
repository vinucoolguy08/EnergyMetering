using EnergyMetering.Models;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

namespace EnergyMetering.Exceptions
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public CustomExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotFoundException ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var logger = _loggerFactory.CreateLogger("ConfigureBuildInExceptionHandler");


            httpContext.Response.ContentType = "application/json";

            var contextRequest = httpContext.Features.Get<IHttpRequestFeature>();

            var response = new ErrorModel()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = ex.Message,
                Path = contextRequest.Path
            }.ToString();

            logger.LogError(response);

            return httpContext.Response.WriteAsync(response);
        }
    }
}
