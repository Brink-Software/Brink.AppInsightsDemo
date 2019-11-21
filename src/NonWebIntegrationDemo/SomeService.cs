using System;
using System.Threading.Tasks;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    public class SomeService
    {
        [AppInsightsDependency(Type = "InteralServiceCall"])
        public static async Task<string> SendAsync(string greeting)
        {
            await Task.Delay(500);
            throw new Exception("Woopsie");

            return greeting;
        }
    }
}
