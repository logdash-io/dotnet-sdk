using Microsoft.AspNetCore.Mvc;

namespace Logdash.AspNetCore.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController(ILogdash logdash) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logdash.Http("test");
        logdash.SetMetric("controller", 1);
        logdash.MutateMetric("controller", 3);
        return Ok();
    }
}