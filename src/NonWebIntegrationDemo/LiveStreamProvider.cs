using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

namespace NonWebIntegrationDemo
{
    internal class LiveStreamProvider
    {
        private readonly TelemetryConfiguration _configuration;

        public LiveStreamProvider(TelemetryConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Enable()
        {
            QuickPulseTelemetryProcessor processor = null;

            var module = new DependencyTrackingTelemetryModule();
            module.Initialize(_configuration);

            _configuration.TelemetryProcessorChainBuilder
                .Use(next =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);
                    return processor;
                })
                .Build();

            var quickPulse = new QuickPulseTelemetryModule();
            quickPulse.Initialize(_configuration);
            quickPulse.RegisterTelemetryProcessor(processor);
        }
    }
}