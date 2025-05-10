using ElisBackend.Presenters.GraphQLSchema;
using GraphQL;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ElisBackend.Presenters.HealthSdlFile
{
    // TODO Move to an endpoint to get SDL file
    public class HealthSchema : IHealthCheck
    {

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            ElisSchema schema = new ElisSchema();
            string sdl = schema.Print();


            if (isHealthy)
            {
                var data = new Dictionary<string, object>() { { "SDL", sdl } };
                return Task.FromResult(
                    new HealthCheckResult(HealthStatus.Healthy, null, null, data) );
            }

            return Task.FromResult(
                new HealthCheckResult(
                    context.Registration.FailureStatus, "Unhealthy no SDL file"));
        }
    }
}
