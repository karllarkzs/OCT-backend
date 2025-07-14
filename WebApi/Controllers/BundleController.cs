using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.Services.Bundles;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BundleController(IBundleService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(BundleDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> AddItems(
        Guid id,
        [FromBody] List<BundleItemDto> items,
        CancellationToken ct
    )
    {
        if (items == null || items.Count == 0)
            return BadRequest("Item list must not be empty.");

        var success = await service.AddProductsAsync(id, items, ct);
        return success ? Ok() : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update(BundleDto dto, CancellationToken ct) =>
        await service.UpdateAsync(dto, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid productId, CancellationToken ct) =>
        await service.RemoveProductAsync(id, productId, ct) ? NoContent() : NotFound();
}
