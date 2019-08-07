using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NonWebIntegrationDemo.Properties;

namespace NonWebIntegrationDemo.AOP
{
    public class ApplicationInsightsLogger : IApplicationInsightsLogger
    {
        private readonly TelemetryClient _telemetryClient;
        private static IApplicationInsightsLogger _instance;
        private static TelemetryConfiguration _telemetryConfiguration;

        private ApplicationInsightsLogger()
        {
            _telemetryClient = new TelemetryClient(DefaultConfiguration);
        }

        public static IApplicationInsightsLogger Instance => _instance ?? (_instance = new ApplicationInsightsLogger());

        public void TrackException(Exception exception)
        { 
            _telemetryClient.TrackException(exception);
        }

        public void TrackTrace(string message, SeverityLevel severityLevel = SeverityLevel.Information)
        {
            _telemetryClient.TrackTrace(message, severityLevel);
        }

        public static TelemetryConfiguration DefaultConfiguration =>
            _telemetryConfiguration ?? (_telemetryConfiguration = new TelemetryConfiguration
            {
                InstrumentationKey = Settings.Default.AppInsightsKey
            });
    }
}