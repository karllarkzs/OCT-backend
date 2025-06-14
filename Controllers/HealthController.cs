using Microsoft.AspNetCore.Mvc;

namespace PharmaBack.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Found");
}
