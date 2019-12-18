using System.Collections.Concurrent;
using System.Threading;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace FunctionApp
{
    public class TelemetryEnrichment : ITelemetryInitializer
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> States = new ConcurrentDictionary<string, AsyncLocal<object>>();

        internal static void SetData(string name, object data) =>
            States.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is ISupportProperties item)) return;

            // Demonstrate use of dynamic properties
            foreach (var (key, state) in States)
            {
                item.Properties[key] = state.Value?.ToString() ?? string.Empty;
            }

            // Demonstrate static property
            item.Properties["Environment"] = "Production";
        }
    }
}