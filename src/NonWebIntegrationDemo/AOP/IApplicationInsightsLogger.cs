using System;
using Microsoft.ApplicationInsights.DataContracts;

namespace NonWebIntegrationDemo.AOP
{
    public interface IApplicationInsightsLogger
    {
        void TrackException(Exception exception);
        void TrackTrace(string message, SeverityLevel severityLevel = SeverityLevel.Information);
    }
}