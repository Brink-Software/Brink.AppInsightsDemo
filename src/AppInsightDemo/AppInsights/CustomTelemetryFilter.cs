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
            // Example: process all telemetry except requests where url contains "SkipThisOne"
            var isRequestToUrlContainingSpecificText = item is RequestTelemetry request && request.Url.ToString().Contains("SkipThisOne");

            if (!isRequestToUrlContainingSpecificText)
                _next.Process(item); // Process the item
            else
            {
                // Item is dropped here
            }
        }
    }
}
