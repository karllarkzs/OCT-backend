namespace PharmaBack.Services.Locations;

using PharmaBack.DTO.Location;
using PharmaBack.Models;

public interface ILocationService
{
    Task<LocationWithProducsDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LocationWithProducsDto>> GetAllAsync(CancellationToken ct = default);
    Task<Location> CreateAsync(LocationDto location, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, string name, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
