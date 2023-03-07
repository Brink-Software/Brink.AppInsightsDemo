using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var MyActivitySource = new ActivitySource(OpenTelemetryProvider.ServiceName);
            using var activity = MyActivitySource.StartActivity("BackgroundService");
            activity.AddEvent(new ActivityEvent("Execution started."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            activity.AddEvent(new ActivityEvent("Execution started."));
        }
    }
}
