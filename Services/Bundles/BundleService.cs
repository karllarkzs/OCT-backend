using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;
using PharmaBack.Models;

namespace PharmaBack.Services.Bundles;

public sealed class BundleService(PharmaDbContext db) : IBundleService
{
    private readonly PharmaDbContext _db = db;

    public async Task<List<BundleDto>> GetAllAsync(CancellationToken ct) =>
        await _db
            .Bundles.Include(b => b.BundleItems)
            .AsNoTracking()
            .Select(b => new BundleDto(
                b.Id,
                b.Barcode,
                b.Name,
                b.Price,
                b.BundleItems != null
                    ? b
                        .BundleItems.Select(i => new BundleItemDto(
                            i.ProductId,
                            i.InventoryBatchId,
                            i.Quantity == 0 ? null : i.Quantity,
                            i.Uses == 0 ? null : i.Uses
                        ))
                        .ToList()
                    : new List<BundleItemDto>()
            ))
            .ToListAsync(ct);

    public async Task<BundleDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var bundle = await _db
            .Bundles.Include(b => b.BundleItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (bundle == null)
            return null;

        return new BundleDto(
            bundle.Id,
            bundle.Barcode,
            bundle.Name,
            bundle.Price,
            bundle.BundleItems != null
                ? bundle
                    .BundleItems.Select(i => new BundleItemDto(
                        i.ProductId,
                        i.InventoryBatchId,
                        i.Quantity == 0 ? null : i.Quantity,
                        i.Uses == 0 ? null : i.Uses
                    ))
                    .ToList()
                : new()
        );
    }

    public async Task<BundleDto> CreateAsync(BundleDto dto, CancellationToken ct)
    {
        if (await _db.Bundles.AnyAsync(b => b.Barcode == dto.Barcode, ct))
            throw new InvalidOperationException($"Bundle code '{dto.Barcode}' already exists.");

        var items = dto.Items is null ? [] : await BuildEntitiesAsync(dto.Items, ct);

        var entity = new Bundle
        {
            Id = dto.Id == default ? Guid.NewGuid() : dto.Id,
            Barcode = dto.Barcode.Trim(),
            Name = dto.Name.Trim(),
            Price = dto.Price,
            BundleItems = items,
        };

        _db.Bundles.Add(entity);
        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(entity.Id, ct)
            ?? throw new InvalidOperationException("Failed to reload created bundle.");
    }

    public async Task<bool> AddProductsAsync(
        Guid bundleId,
        IEnumerable<BundleItemDto> items,
        CancellationToken ct
    )
    {
        var bundle = await _db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.Id == bundleId, ct);
        if (bundle is null)
            return false;

        foreach (var dto in items)
        {
            var product =
                await _db
                    .Products.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId, ct)
                ?? throw new InvalidOperationException($"Product {dto.ProductId} not found.");

            var batch =
                await _db
                    .InventoryBatches.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == dto.InventoryBatchId, ct)
                ?? throw new InvalidOperationException($"Batch {dto.InventoryBatchId} not found.");

            Validate(dto, product);

            var existing = await _db.BundleItems.FirstOrDefaultAsync(
                i =>
                    i.BundleId == bundleId
                    && i.ProductId == dto.ProductId
                    && i.InventoryBatchId == dto.InventoryBatchId,
                ct
            );

            if (existing is null)
            {
                _db.BundleItems.Add(
                    new BundleItem
                    {
                        BundleId = bundleId,
                        ProductId = dto.ProductId,
                        InventoryBatchId = dto.InventoryBatchId,
                        Quantity = dto.Quantity ?? 0,
                        Uses = dto.Uses ?? 0,
                    }
                );
            }
            else
            {
                existing.Quantity = dto.Quantity ?? 0;
                existing.Uses = dto.Uses ?? 0;
                _db.BundleItems.Update(existing);
            }
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateAsync(BundleDto dto, CancellationToken ct)
    {
        var bundle = await _db
            .Bundles.Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.Id == dto.Id, ct);
        if (bundle is null)
            return false;

        bundle.Barcode = dto.Barcode.Trim();
        bundle.Name = dto.Name.Trim();
        bundle.Price = dto.Price;

        var newItems = dto.Items is null ? [] : await BuildEntitiesAsync(dto.Items, ct);

        bundle.BundleItems.Clear();
        foreach (var itm in newItems)
            bundle.BundleItems.Add(itm);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var bundle = await _db.Bundles.FindAsync([id], ct);
        if (bundle is null)
            return false;

        bundle.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveProductAsync(Guid bundleId, Guid productId, CancellationToken ct)
    {
        var row = await _db.BundleItems.FirstOrDefaultAsync(
            i => i.BundleId == bundleId && i.ProductId == productId,
            ct
        );
        if (row is null)
            return false;

        _db.BundleItems.Remove(row);
        await _db.SaveChangesAsync(ct);
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
                await _db
                    .Products.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId, ct)
                ?? throw new InvalidOperationException($"Product {dto.ProductId} not found.");

            var batch =
                await _db
                    .InventoryBatches.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == dto.InventoryBatchId, ct)
                ?? throw new InvalidOperationException($"Batch {dto.InventoryBatchId} not found.");

            Validate(dto, product);

            list.Add(ToEntity(dto));
        }

        return list;
    }

    private static BundleItem ToEntity(BundleItemDto dto) =>
        new()
        {
            ProductId = dto.ProductId,
            InventoryBatchId = dto.InventoryBatchId,
            Quantity = dto.Quantity ?? 0,
            Uses = dto.Uses ?? 0,
        };

    private static void Validate(BundleItemDto dto, Product p)
    {
        var qty = dto.Quantity ?? 0;
        var uses = dto.Uses ?? 0;

        if (!p.IsConsumable)
        {
            if (qty <= 0 || uses > 0)
                throw new ArgumentException(
                    $"Product '{p.Brand}' is non-consumable: Quantity must be > 0 and Uses must be 0."
                );
        }
        else
        {
            if (qty <= 0 && uses <= 0)
                throw new ArgumentException(
                    $"Consumable '{p.Brand}' needs Quantity > 0 or Uses > 0 (or both)."
                );
        }
    }
}
