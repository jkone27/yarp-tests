using Microsoft.AspNetCore.Mvc;
using Travix.Common.Logging;

namespace withTravixCommon.WebService.Controllers
{
    [Route("[controller]")]
    public class ExampleController : Controller
    {
        private readonly ILogger logger;

        public ExampleController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            logger.LogInformation(LogEvent.ExampleEndpointCalled, "The example endpoint was called.");

            return Ok("Hello!");
        }
    }
}
