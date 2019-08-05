using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NonWebIntegrationDemo.Properties;

namespace NonWebIntegrationDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new TelemetryConfiguration
            {
                InstrumentationKey = Settings.Default.AppInsightsKey
            };

            var telemetryClient = new TelemetryClient
            {
                InstrumentationKey = Settings.Default.AppInsightsKey // Put your instrumentation key here
            };

            new LiveStreamProvider(configuration).Enable();

            var pageView = new PageViewTelemetry(nameof(Main));
            var sw = Stopwatch.StartNew();

            var greeting = await new SomeClass(telemetryClient).SayHello("World!");
            Console.WriteLine(greeting);

            pageView.Duration = sw.Elapsed;
            telemetryClient.TrackPageView(pageView);

            telemetryClient.Flush();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
