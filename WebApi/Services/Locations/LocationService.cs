namespace PharmaBack.WebApi.Services.Locations;

using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.Models;
using PharmaBack.WebApi.Services.Crud;

public sealed partial class LocationService(PharmaDbContext db, ICrudService<Location, Guid> crud)
    : ILocationService;
