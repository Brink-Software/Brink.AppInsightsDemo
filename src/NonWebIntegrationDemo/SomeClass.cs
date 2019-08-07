using System;
using System.Threading.Tasks;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    public class SomeClass
    {
        [AppInsightsAdvice]
        public async Task<string> SayHello(string to)
        {
            string response = null;
            
            try
            {
                var greeting = $"Hello {to}";
                ApplicationInsightsLogger.Instance.TrackTrace($"Sending {greeting}");

                response = await new SomeService().SendAsync(greeting);
            }
            catch (Exception exception)
            {
                ApplicationInsightsLogger.Instance.TrackException(exception);
            }

            return response;
        }

        [AppInsightsAdvice]
        public string SaySomething(string text)
        {
            return $"I said '{text}'";
        }
    }
}
