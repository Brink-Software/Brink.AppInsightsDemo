using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading.Tasks;
using AppInsightDemo.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace AppInsightDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly Tracer _tracer;
        private readonly UpDownCounter<int> _meter;

        public ValuesController(ILogger<ValuesController> logger, IBackgroundTaskQueue taskQueue, Tracer tracer, UpDownCounter<int> meter)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _tracer = tracer;
            _meter = meter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo1")]
        public async Task<ActionResult<IEnumerable<string>>> HttpContextFeaturesDemo()
        {
            Tracer.CurrentSpan.SetAttribute("aProperty1", "setUsingActivity");

            Tracer.CurrentSpan.AddEvent("NoProperty1");
            
            using var span = _tracer.StartActiveSpan("aSubOperationOfHttpContextFeaturesDemo");
            
            await Task.Delay(TimeSpan.FromMilliseconds(30));

            return new[] { "value1", "value2" };
        }

        /// <summary>
        /// Adds additional properties to a telemetry of same operation using OTEL
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo2")]
        public ActionResult<IEnumerable<string>> ActivityDemo()
        {
            Activity.Current.AddTag("aProperty2", "setUsingFeature");
            Baggage.SetBaggage("aProperty2", "setUsingActivityBaggage");   // Add aProperty2 to sub operations of the request telemetry item only

            Activity.Current.AddEvent(new ActivityEvent("WithProperty2"));

            var MyActivitySource = new ActivitySource(OpenTelemetryProvider.ServiceName);
            using var activity = MyActivitySource.StartActivity("aSubOperationOfActivityDemo");

            activity.AddEvent(new ActivityEvent("WithProperty2InsideSubOp"));

            return new[] { "value1", "value2" };
        }

        /// <summary>
        /// Adds additional properties to a trace telemetry using log scopes
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo3")]
        public ActionResult<IEnumerable<string>> BeginScopeDemo()
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                {"aProperty3", "setUsingScope"}
            }))
            {
                _logger.LogWarning("Some Warning and a {CustomProperty}", "CustomPropValue");
            }

            return new[] { "value1", "value2" };
        }

        /// <summary>
        /// Adds additional properties to a trace telemetry using log scopes
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo4")]
        public ActionResult<IEnumerable<string>> TrackMetric()
        {
            _meter.Add(1, new KeyValuePair<string, object>("Company", "A"), new KeyValuePair<string, object>("User", "1"));
            _meter.Add(1, new KeyValuePair<string, object>("Company", "A"), new KeyValuePair<string, object>("User", "2"));
            _meter.Add(1, new KeyValuePair<string, object>("Company", "A"), new KeyValuePair<string, object>("User", "1"));
            _meter.Add(1, new KeyValuePair<string, object>("Company", "B"), new KeyValuePair<string, object>("User", "1"));

            return new[] { "value1", "value2" };
        }

        [HttpGet("/api/demo5")]
        public ActionResult TrackWorker()
        {
            var context = Activity.Current.Context;

            _taskQueue.QueueBackgroundWorkItem(async ct =>
            {
                var MyActivitySource = new ActivitySource(OpenTelemetryProvider.ServiceName);
                using var activity = MyActivitySource.StartActivity("QueuedWork", ActivityKind.Internal, context);

                _ = await new HttpClient().GetStringAsync("http://blank.org");

                await Task.Delay(250);

                activity.SetStatus(ActivityStatusCode.Ok, "200");
            });

            return Accepted();
        }

        [HttpGet("/api/demo6")]
        public ActionResult<IEnumerable<string>> TrackException()
        {
            var ex = new Exception("Woops");

            _logger.LogWarning(ex, "Error occured");

            Activity.Current.RecordException(ex);

            return new[] { "value1", "value2" };
        }
    }
}
