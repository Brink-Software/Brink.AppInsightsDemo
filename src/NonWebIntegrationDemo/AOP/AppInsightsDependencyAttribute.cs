using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ArxOne.MrAdvice.Advice;
using ArxOne.MrAdvice.Utility;

namespace NonWebIntegrationDemo.AOP
{
    public sealed class AppInsightsDependencyAttribute : Attribute, IMethodAsyncAdvice
    {
        public string Type { get; set; } = "Other";

        public async Task Advise(MethodAsyncAdviceContext context)
        {
            var parameters = context.TargetMethod.GetParameters();
            var parameterDescription = string.Join(", ",
                parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            var signature = $"{context.Target ?? context.TargetType}.{context.TargetName}({parameterDescription})";

            //using (var operation = TelemetryClient.StartOperation<DependencyTelemetry>(signature))
            {
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
                    //operation.Telemetry.Type = Type;
                    //EnrichRequestTelemetry(operation.Telemetry, context, parameters);
                }
            }
        }
    }
}