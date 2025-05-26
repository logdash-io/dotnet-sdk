using Microsoft.AspNetCore.Mvc;
using LogLevel = Logdash.Models.LogLevel;

namespace Logdash.AspNetCore.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController(ILogdash logdash) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logdash.Log(LogLevel.Http, "test");
        logdash.SetMetric("controller", 1);
        logdash.MutateMetric("controller", 3);
        return Ok();
    }
}