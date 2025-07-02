using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;

namespace PharmaBack.Services.Catalogs;

public sealed class CatalogQuery(PharmaDbContext db) : ICatalogQuery
{
    public async Task<IReadOnlyList<CatalogRowDto>> GetAsync(
        string? search,
        CancellationToken ct = default
    )
    {
        var productStock = await db
            .InventoryBatches.AsNoTracking()
            .GroupBy(b => new { b.ProductId, BatchId = b.Id })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.BatchId,
                Stock = g.Sum(b => b.QuantityOnHand),
            })
            .ToDictionaryAsync(x => (x.ProductId, x.BatchId), x => x.Stock, ct);

        var productRowsQ = db
            .InventoryBatches.AsNoTracking()
            .Where(b => b.QuantityOnHand > 0 && !b.Product.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            productRowsQ = productRowsQ.Where(b =>
                EF.Functions.ILike(b.Product.Barcode, pattern)
                || EF.Functions.ILike(b.Product.Brand, pattern)
            );
        }

        var productRows = await productRowsQ
            .Select(b => new CatalogRowDto(
                b.Id,
                CatalogRowType.Product,
                b.Product.Barcode,
                null,
                b.Product.Generic,
                b.Product.Brand,
                b.Product.Category,
                b.Product.Formulation,
                b.Product.Company,
                b.ExpiryDate,
                b.QuantityOnHand,
                b.Location != null ? b.Location.Name : null,
                b.Product.RetailPrice
            ))
            .ToListAsync(ct);

        var bundlesQ = db.Bundles.AsNoTracking().Where(b => !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            bundlesQ = bundlesQ.Where(b =>
                EF.Functions.ILike(b.Barcode, pattern) || EF.Functions.ILike(b.Name, pattern)
            );
        }

        var rawBundles = await bundlesQ
            .Select(b => new
            {
                b.Id,
                b.Barcode,
                b.Name,
                b.Price,
                b.LocationId,
                LocationName = b.Location != null ? b.Location.Name : null,
                Items = b
                    .BundleItems.Select(i => new
                    {
                        i.ProductId,
                        i.InventoryBatchId,
                        i.Quantity,
                        i.Uses,
                    })
                    .ToList(),
            })
            .ToListAsync(ct);

        var bundleRows = rawBundles
            .Select(b =>
            {
                var buildable = b
                    .Items.Select(i =>
                    {
                        var key = (i.ProductId, i.InventoryBatchId);
                        var stock = productStock.TryGetValue(key, out var qty) ? qty : 0;

                        var denominator = i.Quantity == 0 ? (i.Uses == 0 ? 1 : i.Uses) : i.Quantity;
                        return denominator == 0 ? 0 : stock / denominator;
                    })
                    .DefaultIfEmpty(0)
                    .Min();

                return new CatalogRowDto(
                    b.Id,
                    CatalogRowType.Bundle,
                    b.Barcode,
                    b.Name,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    buildable,
                    b.LocationName,
                    b.Price
                );
            })
            .ToList();

        var allRows = productRows
            .Concat(bundleRows)
            .OrderBy(r => r.Expiry ?? new DateOnly(9999, 12, 31))
            .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return allRows;
    }
}
