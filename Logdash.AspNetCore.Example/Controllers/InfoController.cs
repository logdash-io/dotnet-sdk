using Microsoft.AspNetCore.Mvc;

namespace Logdash.AspNetCore.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController(ILogdashLogger logdash, ILogdashMetrics metrics) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logdash.Http("test");
        metrics.Set("controller", 1);
        metrics.Mutate("controller", 3);
        return Ok();
    }
}