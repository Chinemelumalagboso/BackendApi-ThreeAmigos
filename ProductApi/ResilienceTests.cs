using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Xunit;

namespace ProductApi.ResilienceTests
{
    public class ResilienceTests
    {
        private readonly HttpClient _httpClient;

        public ResilienceTests()
        {
            _httpClient = new HttpClient(new CustomHandler());
        }

        [Fact]
        public async Task TestRetryPolicy()
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var response = await retryPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("test/endpoint");
            });

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task TestCircuitBreakerPolicy()
        {
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30));

            var response = await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("test/failing-endpoint");
            });

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task TestTimeoutPolicy()
        {
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

            var response = await timeoutPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("test/slow-endpoint");
            });

            Assert.True(response.IsSuccessStatusCode);
        }
    }

    public class CustomHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.RequestUri.ToString().Contains("failing-endpoint"))
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else if (request.RequestUri.ToString().Contains("slow-endpoint"))
            {
                await Task.Delay(6000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }
    }
}
