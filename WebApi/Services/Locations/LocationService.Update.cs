namespace PharmaBack.WebApi.Services.Locations;

public sealed partial class LocationService
{
    public Task<bool> UpdateAsync(Guid id, string name, CancellationToken ct = default) =>
        crud.UpdateAsync(id, l => l.Name = name, ct);
}
