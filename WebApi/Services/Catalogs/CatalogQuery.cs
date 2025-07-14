using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.DTO;

namespace PharmaBack.WebApi.Services.Catalogs;

public sealed class CatalogQuery(PharmaDbContext db) : ICatalogQuery
{
    public async Task<IReadOnlyList<CatalogRowDto>> GetAsync(
        string? search,
        CancellationToken ct = default
    )
    {
        var productQ = db.Products.AsNoTracking().Where(p => p.Quantity > 0 && !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            productQ = productQ.Where(p =>
                EF.Functions.ILike(p.Barcode, pattern) || EF.Functions.ILike(p.Brand, pattern)
            );
        }

        var productRows = await productQ
            .Select(p => new CatalogRowDto(
                p.Id,
                CatalogRowType.Product,
                p.Barcode,
                null,
                p.Generic,
                p.Brand,
                p.Category,
                p.Type,
                p.Formulation,
                p.Company,
                p.Quantity,
                p.Location,
                p.RetailPrice
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
                b.Location,
                Items = b.BundleItems.Select(i => new { i.Product.Quantity }).ToList(),
            })
            .ToListAsync(ct);

        var bundleRows = rawBundles
            .Select(b =>
            {
                var buildable = b
                    .Items.Select(i =>
                    {
                        var stock = i.Quantity;
                        return i.Quantity == 0 ? 0 : i.Quantity / i.Quantity;
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
                    b.Location,
                    b.Price
                );
            })
            .ToList();

        var allRows = productRows
            .Concat(bundleRows)
            .OrderBy(r => r.Name ?? r.Brand ?? "", StringComparer.OrdinalIgnoreCase)
            .ToList();

        return allRows;
    }
}
