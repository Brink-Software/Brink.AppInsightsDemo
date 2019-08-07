using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using NonWebIntegrationDemo.AOP;
using NonWebIntegrationDemo.Properties;

namespace NonWebIntegrationDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            new LiveStreamProvider(ApplicationInsightsLogger.DefaultConfiguration).Enable();

            var telemetryClient = new TelemetryClient(ApplicationInsightsLogger.DefaultConfiguration)
            {
                InstrumentationKey = Settings.Default.AppInsightsKey
            };

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();

            while (keyInfo.Key != ConsoleKey.Q)
            {
                var pageView = new PageViewTelemetry(nameof(Main));
                var sw = Stopwatch.StartNew();

                var greeting = await new SomeClass().SayHello("World!");
                Console.WriteLine(greeting);

                var story = new SomeClass().SaySomething("Booh!");
                Console.WriteLine(story);

                pageView.Duration = sw.Elapsed;
                telemetryClient.TrackPageView(pageView);

                telemetryClient.Flush();

                Console.WriteLine("Press any key to restart or Q to quit.");
                keyInfo = Console.ReadKey();
            }
        }
    }
}
