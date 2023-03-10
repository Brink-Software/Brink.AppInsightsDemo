using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppInsightDemo.Middleware;
using AppInsightDemo.Worker;
using OpenTelemetry.Trace;
using System.Diagnostics;

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
            services.AddOpenTelemetry().WithTracing(builder =>
                {
                    builder
                    .AddSource(OpenTelemetryProvider.ServiceName, "1.2.3")
                    .ConfigureResource(builder =>
                    {
                        OpenTelemetryProvider.CreateResourceBuilder();
                    })
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter();
                });

            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();

            services.AddHttpClient();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
        {
            var lifeTimeActivity = new ActivitySource(OpenTelemetryProvider.ServiceName).StartActivity("AppLifetime");

            hostApplicationLifetime.ApplicationStarted.Register(() => { lifeTimeActivity?.AddEvent(new ActivityEvent("App Started")); });
            hostApplicationLifetime.ApplicationStopping.Register(() => { 
                lifeTimeActivity?.AddEvent(new ActivityEvent("App Stopping")); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseMiddleware<CustomMiddleware>();
        }
    }
}
