using OpenTelemetry;
using System.Diagnostics;

namespace FunctionApp
{
    internal class TraceEnrichment : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity activity)
        {
            activity.SetTag("environment.name", "production");
        }
    }
}
