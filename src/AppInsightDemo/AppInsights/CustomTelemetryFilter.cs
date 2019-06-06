using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AppInsightDemo.AppInsights
{
    /// <summary>
    /// A Telemetry filter lets you define whether a telemetry item is dropped or not
    /// </summary>
    public class CustomTelemetryFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public CustomTelemetryFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        { 
            // Example: process all telemetry except fast requests
            if (!(item is RequestTelemetry request) || 
                request.Duration >= TimeSpan.FromMilliseconds(50))
                _next.Process(item); // Call next processor
        }
    }
}
