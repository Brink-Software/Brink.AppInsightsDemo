using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddOpenTelemetry(options =>
                    {
                        var resourceBuilder = ResourceBuilder.CreateDefault();
                        OpenTelemetryProvider.ConfigureResourceBuilder(resourceBuilder);

                        options.IncludeFormattedMessage = true;
                        options.SetResourceBuilder(resourceBuilder);
                        options.AddConsoleExporter();
                        options.AddAzureMonitorLogExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; });
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddOpenTelemetry().WithTracing(builder =>
                        {
                            builder
                            .AddSource(OpenTelemetryProvider.ServiceName, "1.2.3")
                            .ConfigureResource(OpenTelemetryProvider.ConfigureResourceBuilder)
                            .AddConsoleExporter()
                            .AddOtlpExporter()
                            .AddAzureMonitorTraceExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; })
                            .AddProcessor<TraceEnrichment>();
                        });
                        
                    services.AddHostedService<Worker>();
                });
    }

    public class OpenTelemetryProvider
    {
        public static string ServiceName => "ServiceWorker";
        
        public static Action<ResourceBuilder> ConfigureResourceBuilder
        {
            get
            {
                return rb => rb.AddService(ServiceName, serviceNamespace: "OpenTelemetryDemo", serviceInstanceId: Environment.MachineName);
            }
        }
    }
}
