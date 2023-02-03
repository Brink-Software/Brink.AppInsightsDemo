using System.IO;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights.DependencyCollector;

namespace AppInsightDemo.AppInsights
{
    /// <summary>
    /// A telemetry initializer lets you add or modify properties of the telemetry item
    /// before it is send to the App Insights resource
    /// 
    /// You can have multiple of them
    /// </summary>
    public class CustomInitializer : TelemetryInitializerBase
    {
        public CustomInitializer(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        
        }
        
        protected override void OnInitializeTelemetry(HttpContext platformContext, RequestTelemetry requestTelemetry, ITelemetry telemetry)
        {
            telemetry.Context.Component.Version = Assembly.GetCallingAssembly().GetName().Version?.ToString();

            // To add custom properties the item has to implement ISupportProperties
            if (!(telemetry is ISupportProperties supportedTelemetry))
                return;

            supportedTelemetry.Properties["IsPrivilegedCall"] =
                platformContext.User.IsInRole("Admin").ToString();

            // Getting access to the request and the response of a dependency call
            if (telemetry is DependencyTelemetry dependencyTelemetry 
                && dependencyTelemetry.Type.Equals("http", StringComparison.InvariantCultureIgnoreCase)
                && dependencyTelemetry.TryGetHttpResponseOperationDetail(out var response)
                && dependencyTelemetry.TryGetHttpRequestOperationDetail(out var request))
            {
                
            }
        }
    }
    }
}