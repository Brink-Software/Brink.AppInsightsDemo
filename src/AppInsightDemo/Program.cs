using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace AppInsightDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
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
                .UseStartup<Startup>();
    }

    public class OpenTelemetryProvider
    {
        public static string ServiceName => "WebApp";

        public static Action<ResourceBuilder> ConfigureResourceBuilder
        {
            get
            {
                return rb => rb.AddService(ServiceName, serviceNamespace: "OpenTelemetryDemo", serviceInstanceId: Environment.MachineName);
            }
        }
    }
}
