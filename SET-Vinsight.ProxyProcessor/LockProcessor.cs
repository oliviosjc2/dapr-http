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

            return new OkObjectResult(new
            {
                receivedMessage = "message",
                response = $"ProcessLock processed your message!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
