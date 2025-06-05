using Microsoft.AspNetCore.Mvc;

namespace Logdash.AspNetCore.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController(ILogdash logdash, ILogdashMetrics metrics) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logdash.Http("test");
        metrics.SetMetric("controller", 1);
        metrics.MutateMetric("controller", 3);
        return Ok();
    }
}