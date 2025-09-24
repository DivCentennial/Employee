using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Record start time
            var stopwatch = Stopwatch.StartNew();

            // Check if this is a login request
            bool isLoginRequest = context.Request.Path.Value?.Contains("/authentication/login") == true;

            if (isLoginRequest)
            {
                _logger.LogInformation("Login request started at: {StartTime}", DateTime.UtcNow);
            }

            // Continue to next middleware
            await _next(context);

            // Record end time and calculate elapsed time
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;

            if (isLoginRequest)
            {
                _logger.LogInformation("Login request completed in: {ElapsedTime}ms at: {EndTime}",
                    elapsedTime, DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Request to {Path} completed in: {ElapsedTime}ms",
                    context.Request.Path, elapsedTime);
            }
        }
    }
}