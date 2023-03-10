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
                        options.IncludeFormattedMessage = true;
                        options.SetResourceBuilder(OpenTelemetryProvider.CreateResourceBuilder());
                        options.AddConsoleExporter();
                    });
                })
                .UseStartup<Startup>();
    }

    public class OpenTelemetryProvider
    {
        public static string ServiceName => "WebApp";

        public static ResourceBuilder CreateResourceBuilder()
        {
            return ResourceBuilder.CreateDefault()
                                    .AddService(ServiceName, serviceNamespace: "AppInsightDemo", serviceInstanceId: Environment.MachineName);
        }
    }
}
