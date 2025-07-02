using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO;
using PharmaBack.Services.Catalogs;

namespace PharmaBack.Controllers;

[ApiController]
[Route("api/catalog")]
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
