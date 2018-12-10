using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ApplicationInsights;

namespace AppInsightDemo.AppInsights
{
    public sealed class DurationTracker : IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly string _identifier;
        private readonly Stopwatch _stopwatch;

        public DurationTracker(string identifier) : this(new TelemetryClient(), identifier)
        {

        }

        public DurationTracker(TelemetryClient telemetryClient, string identifier)
        {
            _telemetryClient = telemetryClient;
            _identifier = identifier;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _telemetryClient.TrackEvent(
                _identifier,
                metrics: new Dictionary<string, double> { { "durationInMs", _stopwatch.ElapsedMilliseconds } });
        }
    }
}
