using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AppInsightDemo.Middleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Demonstrate how TelemetryClient is injected using DI 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="telemetryClient"></param>
        /// <returns></returns>

        public async Task InvokeAsync(HttpContext context)
        {
            //telemetryClient.TrackTrace($"Middleware {nameof(CustomMiddleware)} invoked");

            await _next(context);
        }
    }
}
