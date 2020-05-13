using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
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

        public async Task InvokeAsync(HttpContext context, TelemetryClient telemetryClient)
        {
            telemetryClient.TrackTrace($"Middleware {nameof(CustomMiddleware)} invoked");

            await _next(context);
        }
    }
}
