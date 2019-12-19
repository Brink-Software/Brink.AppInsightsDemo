using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]

namespace FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ITelemetryInitializer, TelemetryEnrichment>();
            builder.Services.Configure<TelemetryConfiguration>(
                config => {
                    config.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
                });
            builder.Services.AddLogging();
        }
    }
}