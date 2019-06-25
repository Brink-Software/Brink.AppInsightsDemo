using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

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
                .UseApplicationInsights()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    // Add a filter so logging that is piped to application insights is filtered to only log entries with severity
                    // Information or higher. This can be done using a config file as well but this way it allows future modifications to
                    // change the log level at runtime. 
                    logging.AddFilter(level => level >= LogLevel.Information); 
                })
                .UseStartup<Startup>();
    }
}
