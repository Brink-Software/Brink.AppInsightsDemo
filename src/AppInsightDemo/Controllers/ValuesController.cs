using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AppInsightDemo.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppInsightDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public ValuesController(ILogger<ValuesController> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Adds additional properties to a Request Telemetry using HttpContext.Features 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo1")]
        public ActionResult<IEnumerable<string>> HttpContextFeaturesDemo()
        {
            //var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();
            //requestTelemetry.Properties.Add();
            Activity.Current.AddTag("aProperty1", "setUsingActivity");

            //_telemetryClient.TrackEvent("NoProperty1");
            Activity.Current.AddEvent(new ActivityEvent("NoProperty1"));

            return new[] { "value1", "value2" };
        }

        /// <summary>
        /// Adds additional properties to a telemetry of same operation using Activity
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/demo2")]
        public ActionResult<IEnumerable<string>> ActivityDemo()
        {
            //var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();
            //requestTelemetry.Properties.Add("aProperty2", "setUsingFeature");

            Activity.Current.AddBaggage("aProperty2", "setUsingActivityBaggage");   // Add aProperty2 to sub operations of the request telemetry item only

            //_telemetryClient.TrackEvent("WithProperty2");

            //using (_telemetryClient.StartOperation<DependencyTelemetry>("aSubOperationOfActivityDemo"))
            {
                // This dependency telemetry will have only the properties set using Activity.Current.AddBaggage(..)

                // This event telemetry will have only the properties set using Activity.Current.AddBaggage(..)
                //_telemetryClient.TrackEvent("WithProperty2InsideSubOp");
            }

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
            //_telemetryClient.GetMetric("MyCustomMetric", "Company", "User").TrackValue(1, "a", "1");
            //_telemetryClient.GetMetric("MyCustomMetric", "Company", "User").TrackValue(2, "a", "1");
            //_telemetryClient.GetMetric("MyCustomMetric", "Company", "User").TrackValue(3, "a", "1");
            //_telemetryClient.GetMetric("MyCustomMetric", "Company", "User").TrackValue(4, "a", "2");
            //_telemetryClient.GetMetric(new MetricIdentifier("Performance", "MyOtherMetric")).TrackValue(5);

            return new[] { "value1", "value2" };
        }

        [HttpGet("/api/demo5")]
        public ActionResult TrackWorker()
        {
            //var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();

            _taskQueue.QueueBackgroundWorkItem(async ct =>
            {
                //using(var op = _telemetryClient.StartOperation<DependencyTelemetry>("QueuedWork", requestTelemetry.Context.Operation.Id))
                {
                    _ = await new HttpClient().GetStringAsync("http://blank.org");

                    await Task.Delay(250);
                    //op.Telemetry.ResultCode = "200";
                    //op.Telemetry.Success = true;
                }
            });

            return Accepted();
        }

        [HttpGet("/api/demo6")]
        public ActionResult<IEnumerable<string>> TrackException()
        {
            var ex = new Exception("Woops");
            
            _logger.LogWarning(ex, "Error occured");

            return new[] { "value1", "value2" };
        }
    }
}
