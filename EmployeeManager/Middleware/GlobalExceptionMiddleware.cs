using EmployeeManager.Exceptions;
using System.Net;
using System.Text.Json;

namespace EmployeeManager.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                int statusCode = 500;

                if (ex is BadRequestException)
                {
                    statusCode = 400;
                }
                _logger.LogError(ex, "Unhandled exception occurred.");
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    //StatusCode = context.Response.StatusCode,
                    StatusCode = statusCode,
                    Message = "An unexpected error occurred.",
                    Detailed = ex.Message
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}