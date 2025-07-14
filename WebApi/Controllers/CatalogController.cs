using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.Services.Catalogs;

namespace PharmaBack.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CatalogController(ICatalogQuery catalogQuery) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CatalogRowDto>>> GetCatalog(
        [FromQuery] string? search,
        CancellationToken ct
    )
    {
        var results = await catalogQuery.GetAsync(search, ct);
        return Ok(results);
    }
}
