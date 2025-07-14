using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.Services.Products;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<GetProductDto>> GetAll(CancellationToken ct) =>
        await productService.GetAllAsync(ct);

    [HttpPost]
    public async Task<IReadOnlyList<Guid>> Add(
        IEnumerable<CreateProductDto> dtos,
        CancellationToken ct
    ) => await productService.BatchCreateOrUpdateAsync(dtos, ct);

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] EditProductDto dto, CancellationToken ct)
    {
        try
        {
            await productService.EditProductAsync(dto, ct);
            return Ok(new { });
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
    public async Task<IActionResult> Restock([FromBody] RestockProductDto dto, CancellationToken ct)
    {
        try
        {
            await productService.RestockAsync(dto, ct);
            return Ok(new { });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
