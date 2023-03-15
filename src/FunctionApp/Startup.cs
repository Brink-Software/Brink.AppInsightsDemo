using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]

namespace FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLogging()
                .AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    var resourceBuilder = ResourceBuilder.CreateDefault();
                    OpenTelemetryProvider.ConfigureResourceBuilder(resourceBuilder);

                    builder
                        .AddSource(OpenTelemetryProvider.ServiceName, "1.2.3")
                        .SetResourceBuilder(resourceBuilder)
                        .AddConsoleExporter()
                        .AddOtlpExporter()
                        //.AddAzureMonitorTraceExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; })
                        .AddProcessor<TraceEnrichment>();
                })
                .WithMetrics(builder =>
                {
                    builder
                        .AddOtlpExporter()
                        .AddConsoleExporter()
                        //.AddAzureMonitorMetricExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; })
                        .ConfigureResource(OpenTelemetryProvider.ConfigureResourceBuilder);
                });
        }
    }

    public class OpenTelemetryProvider
    {
        public static string ServiceName => "AzFunction";

        public static Action<ResourceBuilder> ConfigureResourceBuilder
        {
            get
            {
                return rb => rb.AddService(ServiceName, serviceNamespace: "OpenTelemetryDemo", serviceInstanceId: Environment.MachineName);
            }
        }
    }
}