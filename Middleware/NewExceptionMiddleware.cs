using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TaskList_Server.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class NewExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<NewExceptionMiddleware> _logger;
        public NewExceptionMiddleware(RequestDelegate next, ILogger<NewExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // call the next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong: {ex.Message}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Please try again later.",
                Detailed = ex.Message // ⚠️ in prod, hide details
            }.ToString()!);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    //public static class NewExceptionMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseNewExceptionMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<NewExceptionMiddleware>();
    //    }
    //}
}
