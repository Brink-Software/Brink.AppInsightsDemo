using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppInsightDemo.Middleware;
using AppInsightDemo.Worker;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppInsightDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var meter = new Meter(OpenTelemetryProvider.ServiceName);
            var customMetric = meter.CreateUpDownCounter<int>("MyCustomMetric", "feet", "Some description");

            services.AddSingleton(customMetric);

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                    {
                        builder
                            .AddSource(OpenTelemetryProvider.ServiceName, "1.2.3")
                            .ConfigureResource(OpenTelemetryProvider.ConfigureResourceBuilder)
                            .AddHttpClientInstrumentation()
                            .AddAspNetCoreInstrumentation()
                            .AddConsoleExporter()
                            .AddOtlpExporter()
                            .AddAzureMonitorTraceExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; })
                            .AddProcessor<TraceEnrichment>();
                    })
                .WithMetrics(builder =>
                {
                    builder
                        .AddMeter(meter.Name)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddOtlpExporter()
                        .AddConsoleExporter()
                        .AddAzureMonitorMetricExporter(options => { options.ConnectionString = "InstrumentationKey=3ba43954-2b8e-4588-bdc4-ea371255bb27;IngestionEndpoint=https://westeurope-3.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"; })
                        .ConfigureResource(OpenTelemetryProvider.ConfigureResourceBuilder);
                });
            services.AddSingleton(TracerProvider.Default.GetTracer(OpenTelemetryProvider.ServiceName));

            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();

            services.AddHttpClient();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime, Tracer tracer, ILogger<Startup> logger)
        {
            var lifeTimeSpan = tracer.StartRootSpan("AppLifetime");

            hostApplicationLifetime.ApplicationStarted.Register(() => {
                lifeTimeSpan?.AddEvent("App Started");
                logger.LogInformation("App Started");
            });
            hostApplicationLifetime.ApplicationStopping.Register(async () => {
                lifeTimeSpan?.AddEvent("App Stopping");
                logger.LogInformation("App Stopping");
                lifeTimeSpan.Dispose();
                await Task.Delay(1000);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseMiddleware<CustomMiddleware>();
        }
    }
}
