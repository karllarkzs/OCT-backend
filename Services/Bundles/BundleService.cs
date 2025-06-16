using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;
using PharmaBack.Models;

namespace PharmaBack.Services.Bundles;

public class BundleService(PharmaDbContext db) : IBundleService
{
    public async Task<List<BundleDto>> GetAllAsync(CancellationToken ct) =>
        await db
            .Bundles.Include(b => b.BundleItems)
            .Select(b => new BundleDto(
                b.BundleId,
                b.Code,
                b.Name,
                b.Price,
                b.BundleItems.Select(i => new BundleItemDto(i.ProductId, i.Quantity)).ToList()
            ))
            .ToListAsync(ct);

    public async Task<BundleDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var b = await db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.BundleId == id, ct);

        return b is null
            ? null
            : new BundleDto(
                b.BundleId,
                b.Code,
                b.Name,
                b.Price,
                b.BundleItems.Select(i => new BundleItemDto(i.ProductId, i.Quantity)).ToList()
            );
    }

    public async Task<BundleDto> CreateAsync(BundleDto dto, CancellationToken ct)
    {
        var bundle = new Bundle
        {
            BundleId = dto.BundleId != default ? dto.BundleId : Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Price = dto.Price,
            BundleItems = dto
                .Items.Select(i => new BundleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                })
                .ToList(),
        };
        db.Bundles.Add(bundle);
        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(bundle.BundleId, ct)
            ?? throw new Exception("Created bundle not found");
    }

    public async Task<bool> UpdateAsync(Guid id, BundleDto dto, CancellationToken ct)
    {
        var bundle = await db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.BundleId == id, ct);
        if (bundle == null)
            return false;

        bundle.Code = dto.Code;
        bundle.Name = dto.Name;
        bundle.Price = dto.Price;

        // optionally sync items here or leave as-is
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var bundle = await db.Bundles.FindAsync([id], ct);
        if (bundle == null)
            return false;

        db.Bundles.Remove(bundle);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddProductAsync(Guid bundleId, BundleItemDto item, CancellationToken ct)
    {
        var bundle = await db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.BundleId == bundleId, ct);
        if (bundle == null)
            return false;

        var existing = bundle.BundleItems.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing != null)
        {
            existing.Quantity = item.Quantity; // update quantity
        }
        else
        {
            bundle.BundleItems.Add(
                new BundleItem { ProductId = item.ProductId, Quantity = item.Quantity }
            );
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveProductAsync(Guid bundleId, Guid productId, CancellationToken ct)
    {
        var item = await db.BundleItems.FirstOrDefaultAsync(
            i => i.BundleId == bundleId && i.ProductId == productId,
            ct
        );

        if (item == null)
            return false;

        db.BundleItems.Remove(item);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
