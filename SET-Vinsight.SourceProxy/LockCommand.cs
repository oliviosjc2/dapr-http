using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SET_Vinsight.SourceProxy
{
    public class LockCommand
    {
        private readonly ILogger<LockCommand> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500";

        public LockCommand(ILogger<LockCommand> logger)
        {
            _logger = logger;
        }

        [Function("lock")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic? data = JsonConvert.DeserializeObject(requestBody);

            var messageData = new
            {
                message = data?.message ?? "Object created inside SET-Vinsight.SourceProxy :: LockCommand"
            };

            var daprUrl = $"http://localhost:{daprPort}/v1.0/invoke/proxy-processor/method/api/process-lock";
            var content = new StringContent(JsonConvert.SerializeObject(messageData), Encoding.UTF8, "application/json");

            _logger.LogInformation($"Calling ProxyProcessor via Dapr at: {daprUrl}");

            HttpResponseMessage response = await _httpClient.PostAsync(daprUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response from ProxyProcessor: {responseBody}");

            return new OkObjectResult(new
            {
                originalRequest = data,
                function2Response = JsonConvert.DeserializeObject(responseBody)
            });
        }
    }
}
