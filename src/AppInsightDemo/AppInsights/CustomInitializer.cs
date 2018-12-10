using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace AppInsightDemo.AppInsights
{
    public class CustomInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomInitializer(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is ISupportProperties supportedTelemetry))
                return;

            supportedTelemetry.Properties["IsPrivilegedCall"] =
                _contextAccessor.HttpContext.User.IsInRole("Admin").ToString();
        }
    }
}