using System;
using Microsoft.ApplicationInsights.Extensibility;
using NonWebIntegrationDemo.Properties;

namespace NonWebIntegrationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new TelemetryConfiguration
            {
                InstrumentationKey = Settings.Default.AppInsightsKey
            };

            new LiveStreamProvider(configuration).Enable();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
