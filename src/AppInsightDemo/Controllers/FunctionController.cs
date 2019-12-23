using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppInsightDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ValuesController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public FunctionController(IHttpClientFactory clientFactory, ILogger<ValuesController> logger, TelemetryClient telemetryClient)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            _logger.LogInformation("Invoking function");

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:7071/api/HttpTriggered");

            _telemetryClient.TrackEvent($"Called function with result code {response.StatusCode}");

            return response.IsSuccessStatusCode ? Ok() : (ActionResult)Problem();
        }
    }
}