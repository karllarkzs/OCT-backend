using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO.Location;
using PharmaBack.Models;
using PharmaBack.Services.Locations;

namespace PharmaBack.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController(ILocationService svc) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<LocationWithProducsDto?> Get(Guid id, CancellationToken ct) =>
        await svc.GetAsync(id, ct);

    [HttpGet]
    public async Task<IReadOnlyList<LocationWithProducsDto>> GetAll(CancellationToken ct) =>
        await svc.GetAllAsync(ct);

    [HttpPost]
    public async Task<Location> Create(LocationDto location, CancellationToken ct) =>
        await svc.CreateAsync(location, ct);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, LocationDto dto, CancellationToken ct)
    {
        var ok = await svc.UpdateAsync(id, dto.Name, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await svc.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
