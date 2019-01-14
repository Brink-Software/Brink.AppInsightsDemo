using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer;
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

            var telemetryClient = new TelemetryClient
            {
                InstrumentationKey = Settings.Default.AppInsightsKey // Put your instrumentation key here
            };


            new UnobservedExceptionTelemetryModule().Initialize(configuration);
            new FirstChanceExceptionStatisticsTelemetryModule().Initialize(configuration);
            new UnhandledExceptionTelemetryModule().Initialize(configuration);

            new LiveStreamProvider(configuration).Enable();

            var greeting = new SomeClass(telemetryClient).SayHello("World");
            Console.WriteLine(greeting);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }

    public class SomeClass
    {
        private readonly TelemetryClient _telemetryClient;

        public SomeClass(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public string SayHello(string to)
        {
            string response = null;

            var sw = Stopwatch.StartNew(); // We need this to measure the method duration
            try
            {
                var greeting = $"Hello {to}";
                _telemetryClient.TrackTrace($"Sending {greeting}");

                response = new SomeService().Send(greeting);
            }
            catch (Exception exception)
            {
                _telemetryClient.TrackException(exception);
            }

            CreateAndTrackRequest(sw.Elapsed); // Track a successful request
            return response;
        }

        private void CreateAndTrackRequest(TimeSpan duration, [CallerMemberName]string name = "")
        {
            var request = new RequestTelemetry
            {
                Timestamp = DateTime.UtcNow.Subtract(duration),
                Name = name,
                Duration = duration,
                Success = true,
                ResponseCode = "OK"
            };

            _telemetryClient.TrackRequest(request);
        }
    }

    public class SomeService
    {
        public string Send(string greeting)
        {
            throw new Exception("Woops");
        }
    }
}
