using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Bundles;

public sealed class BundleService(PharmaDbContext db) : IBundleService
{
    public async Task<List<BundleDto>> GetAllAsync(CancellationToken ct)
    {
        var bundles = await db
            .Bundles.Include(b => b.BundleItems)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .ToListAsync(ct);

        return bundles
            .Select(b => new BundleDto(
                b.Id,
                b.Barcode,
                b.Name,
                b.Price,
                b.Location,
                b.BundleItems.Select(i => new BundleItemDto(
                        i.ProductId,
                        i.Quantity == 0 ? null : i.Quantity,
                        new CatalogRowDto(
                            i.Product.Id,
                            CatalogRowType.Product,
                            i.Product.Barcode,
                            null,
                            i.Product.Generic,
                            i.Product.Brand,
                            i.Product.Category,
                            i.Product.Type,
                            i.Product.Formulation,
                            i.Product.Company,
                            i.Product.Quantity,
                            i.Product.Location,
                            i.Product.RetailPrice
                        )
                    ))
                    .ToList()
            ))
            .ToList();
    }

    public async Task<BundleDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var bundle = await db
            .Bundles.Include(b => b.BundleItems)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct);

        if (bundle is null)
            return null;

        return new BundleDto(
            bundle.Id,
            bundle.Barcode,
            bundle.Name,
            bundle.Price,
            bundle.Location,
            bundle
                .BundleItems.Select(i => new BundleItemDto(
                    i.ProductId,
                    i.Quantity == 0 ? null : i.Quantity,
                    new CatalogRowDto(
                        i.Product.Id,
                        CatalogRowType.Product,
                        i.Product.Barcode,
                        null,
                        i.Product.Generic,
                        i.Product.Brand,
                        i.Product.Category,
                        i.Product.Type,
                        i.Product.Formulation,
                        i.Product.Company,
                        i.Product.Quantity,
                        i.Product.Location,
                        i.Product.RetailPrice
                    )
                ))
                .ToList()
        );
    }

    public async Task<BundleDto> CreateAsync(BundleDto dto, CancellationToken ct)
    {
        if (await db.Bundles.AnyAsync(b => b.Barcode == dto.Barcode, ct))
            throw new InvalidOperationException($"Bundle code '{dto.Barcode}' already exists.");

        var items = dto.Items is null ? [] : await BuildEntitiesAsync(dto.Items, ct);

        var entity = new Bundle
        {
            Id = dto.Id == default ? Guid.NewGuid() : dto.Id,
            Barcode = dto.Barcode.Trim(),
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Location = dto.Location?.Trim(),
            BundleItems = items,
        };

        db.Bundles.Add(entity);
        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(entity.Id, ct)
            ?? throw new InvalidOperationException("Failed to reload created bundle.");
    }

    public async Task<bool> UpdateAsync(BundleDto dto, CancellationToken ct)
    {
        var bundle = await db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.Id == dto.Id, ct);

        if (bundle is null)
            return false;

        bundle.Barcode = dto.Barcode.Trim();
        bundle.Name = dto.Name.Trim();
        bundle.Price = dto.Price;
        bundle.Location = dto.Location?.Trim();

        var newItems = dto.Items is null ? [] : await BuildEntitiesAsync(dto.Items, ct);

        bundle.BundleItems.Clear();
        foreach (var itm in newItems)
            bundle.BundleItems.Add(itm);

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var bundle = await db.Bundles.FindAsync([id], ct);
        if (bundle is null)
            return false;

        bundle.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddProductsAsync(
        Guid bundleId,
        IEnumerable<BundleItemDto> items,
        CancellationToken ct
    )
    {
        var bundle = await db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.Id == bundleId, ct);

        if (bundle is null)
            return false;

        foreach (var dto in items)
        {
            var product =
                await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dto.ProductId, ct)
                ?? throw new InvalidOperationException($"Product {dto.ProductId} not found.");

            Validate(dto, product);

            var existing = await db.BundleItems.FirstOrDefaultAsync(
                i => i.BundleId == bundleId && i.ProductId == dto.ProductId,
                ct
            );

            if (existing is null)
            {
                db.BundleItems.Add(
                    new BundleItem
                    {
                        BundleId = bundleId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity ?? 0,
                    }
                );
            }
            else
            {
                existing.Quantity = dto.Quantity ?? 0;
                db.BundleItems.Update(existing);
            }
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveProductAsync(Guid bundleId, Guid productId, CancellationToken ct)
    {
        var row = await db.BundleItems.FirstOrDefaultAsync(
            i => i.BundleId == bundleId && i.ProductId == productId,
            ct
        );

        if (row is null)
            return false;

        db.BundleItems.Remove(row);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<List<BundleItem>> BuildEntitiesAsync(
        IEnumerable<BundleItemDto> dtos,
        CancellationToken ct
    )
    {
        var list = new List<BundleItem>();

        foreach (var dto in dtos)
        {
            var product =
                await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dto.ProductId, ct)
                ?? throw new InvalidOperationException($"Product {dto.ProductId} not found.");

            Validate(dto, product);

            list.Add(new BundleItem { ProductId = dto.ProductId, Quantity = dto.Quantity ?? 0 });
        }

        return list;
    }

    private static void Validate(BundleItemDto dto, Product p)
    {
        var qty = dto.Quantity ?? 0;
        if (qty <= 0)
            throw new ArgumentException($"Product '{p.Brand}' quantity must be > 0");
    }
}
