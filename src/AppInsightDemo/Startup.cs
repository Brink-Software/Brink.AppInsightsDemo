using System;
using System.Threading;
using AppInsightDemo.AppInsights;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();

            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer, CustomInitializer>();
            services.AddApplicationInsightsTelemetryProcessor<CustomTelemetryFilter>();
            services.Configure<SnapshotCollectorConfiguration>(Configuration.GetSection(nameof(SnapshotCollectorConfiguration)));
            services.AddApplicationInsightsTelemetryProcessor<SnapshotCollectorTelemetryProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime, TelemetryClient telemetryClient)
        {
            hostApplicationLifetime.ApplicationStarted.Register(() => { telemetryClient.TrackEvent("App Started"); });
            hostApplicationLifetime.ApplicationStopping.Register(() => { telemetryClient.TrackEvent("App Stopping"); });
            hostApplicationLifetime.ApplicationStopped.Register(() => {
                telemetryClient.TrackEvent("App Stopped");
                telemetryClient.Flush();

                Thread.Sleep(TimeSpan.FromSeconds(5));
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
