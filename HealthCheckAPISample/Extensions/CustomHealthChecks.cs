using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckAPISample.Extensions
{
    public class CustomHealthChecksWithArgs : IHealthCheck
    {
        private readonly string _envUri;
        private readonly string _apiName;

        public CustomHealthChecksWithArgs(string envUri, string apiName)
            => (_envUri, _apiName) = (envUri, apiName);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var catUrl = _envUri;

            var client = new HttpClient();
            client.BaseAddress = new Uri(catUrl + _apiName);

            HttpResponseMessage response = await client.GetAsync("");

            return response.StatusCode == HttpStatusCode.OK ?
                await Task.FromResult(new HealthCheckResult(
                      status: HealthStatus.Healthy,
                      description: "Healthy")) :
                await Task.FromResult(new HealthCheckResult(
                      status: HealthStatus.Unhealthy,
                      description: "Possible Issue"));
        }
    }
}