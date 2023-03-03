using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ArxOne.MrAdvice.Advice;
namespace NonWebIntegrationDemo.AOP
{
    public sealed class AppInsightsRequestAttribute : Attribute, IMethodAsyncAdvice
    {
        public async Task Advise(MethodAsyncAdviceContext context)
        {
            var parameters = context.TargetMethod.GetParameters();
            var parameterDescription = string.Join(", ",
                parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            var signature = $"{context.Target ?? context.TargetType}.{context.TargetName}({parameterDescription})";

            //using (var operation = TelemetryClient.StartOperation<RequestTelemetry>(signature))
            //{
                try
                {
                    await context.ProceedAsync();
                }
                catch (Exception)
                {
                    //operation.Telemetry.Success = false;
                    throw;
                }
                finally
                {
                    //EnrichRequestTelemetry(operation.Telemetry, context, parameters);
               }
            //
        }
    }
}