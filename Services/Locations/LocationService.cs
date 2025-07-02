namespace PharmaBack.Services.Locations;

using PharmaBack.Data;
using PharmaBack.Models;
using PharmaBack.Services.Crud;

public sealed partial class LocationService(PharmaDbContext db, ICrudService<Location, Guid> crud)
    : ILocationService;
