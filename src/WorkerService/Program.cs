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
                        options.IncludeFormattedMessage = true;
                        options.SetResourceBuilder(OpenTelemetryProvider.CreateResourceBuilder());
                        options.AddConsoleExporter();
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddOpenTelemetry().WithTracing(builder =>
                        {
                            builder
                            .AddSource(OpenTelemetryProvider.ServiceName, "1.2.3")
                            .ConfigureResource(builder =>
                            {
                                OpenTelemetryProvider.CreateResourceBuilder();
                            })
                            .AddConsoleExporter()
                            .AddProcessor<TraceEnrichment>();
                        });
                        
                    services.AddHostedService<Worker>();
                });
    }

    public class OpenTelemetryProvider
    {
        public static string ServiceName => "ServiceWorker";

        public static ResourceBuilder CreateResourceBuilder()
        {
            return ResourceBuilder.CreateDefault()
                                    .AddService(ServiceName, serviceNamespace: "AppInsightDemo", serviceInstanceId: Environment.MachineName);
        }
    }
}
