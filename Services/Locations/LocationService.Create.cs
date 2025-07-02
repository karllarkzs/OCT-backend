namespace PharmaBack.Services.Locations;

using PharmaBack.DTO.Location;
using PharmaBack.Models;

public sealed partial class LocationService
{
    public async Task<Location> CreateAsync(LocationDto location, CancellationToken ct = default)
    {
        Location newLocation = new() { Name = location.Name };
        db.Locations.Add(newLocation);
        await db.SaveChangesAsync(ct);
        return newLocation;
    }
}
