using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ArxOne.MrAdvice.Advice;
using ArxOne.MrAdvice.Utility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace NonWebIntegrationDemo.AOP
{
    public sealed class AppInsightsDependencyAttribute : Attribute, IMethodAsyncAdvice
    {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient(TelemetryConfiguration.Active);

        public async Task Advise(MethodAsyncAdviceContext context)
        {
            var parameters = context.TargetMethod.GetParameters();
            var parameterDescription = string.Join(", ",
                parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            var signature = $"{context.Target}.{context.TargetName}({parameterDescription})";

            using (var operation = TelemetryClient.StartOperation<DependencyTelemetry>(signature))
            {
                try
                {
                    await context.ProceedAsync();
                }
                catch (Exception)
                {
                    operation.Telemetry.Success = false;
                    throw;
                }
                finally
                {
                    EnrichRequestTelemetry(operation.Telemetry, context, parameters);
                }
            }
        }

        private static void EnrichRequestTelemetry(ISupportProperties telemetry, MethodAsyncAdviceContext context, IReadOnlyList<ParameterInfo> parameters)
        {
            telemetry.Properties.Add(
                new KeyValuePair<string, string>("Accessibility",
                    context.TargetMethod.Attributes.ToVisibilityScope().ToString()));

            for (var i = 0; i < context.Arguments.Count; i++)
            {
                telemetry.Properties.Add($"ARG {parameters[i].Name}", context.Arguments[i].ToString());
            }
        }
    }
}