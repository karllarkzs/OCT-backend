using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO;
using PharmaBack.Services.Bundles;

namespace PharmaBack.Controllers;

[ApiController]
[Route("api/bundles")]
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
        return CreatedAtAction(nameof(GetById), new { id = created.BundleId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, BundleDto dto, CancellationToken ct) =>
        await service.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, BundleItemDto dto, CancellationToken ct) =>
        await service.AddProductAsync(id, dto, ct) ? Ok() : NotFound();

    [HttpDelete("{id:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid productId, CancellationToken ct) =>
        await service.RemoveProductAsync(id, productId, ct) ? NoContent() : NotFound();
}
