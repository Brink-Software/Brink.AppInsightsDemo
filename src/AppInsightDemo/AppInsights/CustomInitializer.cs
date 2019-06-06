using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace AppInsightDemo.AppInsights
{
    /// <summary>
    /// A telemetry initializer lets you add or modify properties of the telemetry item
    /// before it is send to the App Insights resource
    /// 
    /// You can have multiple of them
    /// </summary>
    public class CustomInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomInitializer(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Device.Model = "Web Server";

            // To add custom properties the item has to implement ISupportProperties
            if (!(telemetry is ISupportProperties supportedTelemetry))
                return;

            supportedTelemetry.Properties["IsPrivilegedCall"] =
                _contextAccessor.HttpContext.User.IsInRole("Admin").ToString();
        }
    }
}