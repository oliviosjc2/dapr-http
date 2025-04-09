using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SET_Vinsight.ProxyProcessor
{
    public class LockProcessor
    {
        private readonly ILogger<LockProcessor> _logger;

        public LockProcessor(ILogger<LockProcessor> logger)
        {
            _logger = logger;
        }

        [Function("process-lock")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("ProcessLock received a request");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic? data = JsonConvert.DeserializeObject(requestBody);

            string? message = data?.message;
            _logger.LogInformation($"Received message: {message}");

            return new OkObjectResult(new
            {
                receivedMessage = message,
                response = $"ProcessLock processed your message: {message}",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
