using DeliveryProductAPI.Server.Dtos;
using System.Net;

namespace DeliveryProductAPI.Server.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(
                    ex.Message,
                    "Internal Server Error",
                    httpContext,
                    HttpStatusCode.InternalServerError
                    );
            }
        }

        public async Task HandleExceptionAsync(
            string exMsg,
            string msg,
            HttpContext httpContext,
            HttpStatusCode statusCode
            )
        {
            _logger.LogError(exMsg);

            HttpResponse response = httpContext.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            await response.WriteAsJsonAsync(new HttpErrorDto { Message = msg, StatusCode = (int)statusCode }.ToString());
        }
    }
}
