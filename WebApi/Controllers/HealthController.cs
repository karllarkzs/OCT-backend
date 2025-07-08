using Microsoft.AspNetCore.Mvc;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Found");
}
