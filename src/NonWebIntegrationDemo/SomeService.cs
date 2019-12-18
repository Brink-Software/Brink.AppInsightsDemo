using System;
using System.Threading.Tasks;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    [AppInsightsDependency(Type = "InternalServiceCall")]
    public static class SomeService
    {
        public static async Task<string> SendAsync(string greeting)
        {
            await Task.Delay(500);
            throw new InvalidOperationException("Woopsie");

            return greeting;
        }
    }
}
