using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.DTO.Restock;
using PharmaBack.WebApi.Services.Products;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<GetProductDto>> GetAll(
        [FromQuery] bool includeHistory = false,
        CancellationToken ct = default
    ) => await productService.GetAllAsync(includeHistory, ct);

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<Guid>>> Add(
        IEnumerable<CreateProductDto> dtos,
        CancellationToken ct
    )
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            return Unauthorized();

        var result = await productService.BatchCreateOrUpdateAsync(dtos, userId, userName, ct);
        return Ok(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Edit(
        [FromBody] IEnumerable<EditProductDto> dtos,
        CancellationToken ct
    )
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            return Unauthorized();

        try
        {
            var updatedIds = await productService.EditProductAsync(dtos, userId, userName, ct);
            return Ok(new { updated = updatedIds });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("restock")]
    [Authorize]
    public async Task<IActionResult> Restock([FromBody] CreateRestockDto dto, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            return Unauthorized();

        try
        {
            var result = await productService.RestockAsync(dto, userId, userName, ct);
            return Ok(new { restockId = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("locations")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetLocations(CancellationToken ct)
    {
        var locations = await productService.GetAllLocationsAsync(ct);
        return Ok(locations);
    }

    [HttpGet("companies")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCompanies(CancellationToken ct)
    {
        var companies = await productService.GetAllCompaniesAsync(ct);
        return Ok(companies);
    }
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> SoftDeleteMany([FromBody] List<Guid> ids, CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            return Unauthorized();

        try
        {
            await productService.SoftDeleteAsync(ids, userId, userName, ct);
            return Ok(new { deleted = ids });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}
