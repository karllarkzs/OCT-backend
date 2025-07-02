namespace PharmaBack.Services.Locations;

using Microsoft.EntityFrameworkCore;
using PharmaBack.DTO.Location;
using PharmaBack.DTO.Product;

public sealed partial class LocationService
{
    public async Task<LocationWithProducsDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var loc = await db
            .Locations.Include(l => l.InventoryBatches)
            .ThenInclude(b => b.Product)
            .ThenInclude(p => p.Consumable)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (loc is null)
            return null;

        var products = loc
            .InventoryBatches.Where(b => !b.Product.IsDeleted)
            .Select(b => new GetProductDto(
                b.ProductId,
                b.Id,
                b.Product.Barcode,
                b.Product.Brand,
                b.Product.Generic,
                b.Product.Category,
                b.Product.Formulation,
                b.Product.Company,
                b.Product.RetailPrice,
                b.Product.WholesalePrice,
                b.QuantityOnHand,
                b.ExpiryDate,
                loc.Name,
                loc.Id,
                b.Product.IsConsumable,
                b.Product.Consumable?.UsesMax,
                b.Product.Consumable?.UsesLeft
            ))
            .ToList();

        return new LocationWithProducsDto(loc.Id, loc.Name, products);
    }
}
