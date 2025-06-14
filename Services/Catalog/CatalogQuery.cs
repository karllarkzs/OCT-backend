namespace PharmaBack.Services.Catalog;

using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;

public sealed class CatalogQuery(PharmaDbContext db) : ICatalogQuery
{
    public async Task<IReadOnlyList<CatalogRowDto>> GetAsync(string? search, CancellationToken ct)
    {
        var lots = db
            .InventoryBatches.Where(b => b.QuantityOnHand > 0)
            .Select(b => new CatalogRowDto(
                b.BatchId,
                "PRODUCT",
                b.Product.Barcode,
                b.Product.Name,
                b.ExpiryDate,
                b.QuantityOnHand,
                b.StorageLocation,
                null
            ));

        var bundles = db
            .Bundles.Where(b => !b.IsDeleted)
            .Select(b => new CatalogRowDto(
                b.BundleId,
                "BUNDLE",
                b.Code,
                b.Name,
                null,
                b.BundleItems.Select(i =>
                        db.InventoryBatches.Where(bb => bb.ProductId == i.ProductId)
                            .Sum(bb => bb.QuantityOnHand) / i.Quantity
                    )
                    .Min(),
                null,
                b.Price
            ));

        var query = lots.Concat(bundles);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r =>
                EF.Functions.ILike(r.Code, $"%{search}%")
                || EF.Functions.ILike(r.Name, $"%{search}%")
            );

        return await query
            .OrderBy(r => r.Expiry ?? DateTime.MaxValue)
            .ThenBy(r => r.Name)
            .ToListAsync(ct);
    }
}
