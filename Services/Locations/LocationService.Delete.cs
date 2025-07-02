namespace PharmaBack.Services.Locations;

public sealed partial class LocationService
{
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) =>
        crud.DeleteAsync(id, ct);
}
