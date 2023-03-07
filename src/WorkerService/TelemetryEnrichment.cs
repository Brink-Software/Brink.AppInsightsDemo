using OpenTelemetry;
using System.Diagnostics;

namespace WorkerService
{
    internal class TraceEnrichment : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity activity)
        {
            activity.SetTag("environment.name", "production");
        }
    }
}
