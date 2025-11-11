using Byway.CourseManagement.Algoriza.API.Errors;

namespace Byway.CourseManagement.Algoriza.API.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate? next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IWebHostEnvironment _env = env;


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                await _next.Invoke(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                    ? new ApiExceptionResponse(StatusCodes.Status500InternalServerError, ex.Message, ex.StackTrace?.ToString())
                    : new ApiResponse(StatusCodes.Status500InternalServerError);

                await context.Response.WriteAsJsonAsync(response);
            }
        }

    }
}
