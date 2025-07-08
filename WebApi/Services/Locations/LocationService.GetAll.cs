namespace PharmaBack.WebApi.Services.Locations;

using Microsoft.EntityFrameworkCore;
using PharmaBack.DTO.Location;
using PharmaBack.DTO.Product;

public sealed partial class LocationService
{
    public async Task<IReadOnlyList<LocationWithProducsDto>> GetAllAsync(
        CancellationToken ct = default
    )
    {
        var list = await db
            .Locations.Include(l => l.InventoryBatches)
            .ThenInclude(b => b.Product)
            .ThenInclude(p => p.Consumable)
            .ToListAsync(ct);

        return
        [
            .. list.Select(l => new LocationWithProducsDto(
                l.Id,
                l.Name,
                [
                    .. l
                        .InventoryBatches.Where(b => !b.Product.IsDeleted)
                        .Select(b => new GetProductDto(
                            b.Product.Id,
                            b.Id,
                            b.Product.Barcode,
                            b.Product.Generic,
                            b.Product.Brand,
                            b.Product.Category,
                            b.Product.Formulation,
                            b.Product.Company,
                            b.Product.RetailPrice,
                            b.Product.WholesalePrice,
                            b.QuantityOnHand,
                            b.ExpiryDate,
                            l.Name,
                            l.Id,
                            b.Product.IsConsumable,
                            b.Product.Consumable?.UsesMax,
                            b.Product.Consumable?.UsesLeft
                        )),
                ]
            )),
        ];
    }
}
