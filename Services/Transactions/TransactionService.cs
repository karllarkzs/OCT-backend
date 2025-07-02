using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;
using PharmaBack.DTO.Transactions;
using PharmaBack.Models;

namespace PharmaBack.Services.Transactions;

public sealed class TransactionService(PharmaDbContext db) : ITransactionService
{
    public async Task<Guid> ProcessAsync(TransactionCreateDto dto, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var tx = new Transaction
        {
            Subtotal = dto.Subtotal,
            Vat = dto.Vat,
            SpecialDiscount = dto.SpecialDiscount,
            Total = dto.Total,
            DiscountId = dto.DiscountId,
        };

        db.Transactions.Add(tx);

        foreach (var item in dto.Items)
        {
            var itemType = item.ItemType ?? await InferItemTypeAsync(item.CatalogId, ct);

            switch (itemType)
            {
                case CatalogRowType.Product:
                    await ProcessProduct(tx, item, ct);
                    break;

                case CatalogRowType.Bundle:
                    await ProcessBundle(tx, item, ct);
                    break;

                default:
                    throw new InvalidOperationException("Invalid item type.");
            }
        }

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return tx.Id;
    }

    public async Task<TransactionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var tx = await db
            .Transactions.Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (tx is null)
            return null;

        return new TransactionDetailDto(
            tx.Id,
            tx.CreatedAt,
            tx.Subtotal,
            tx.Vat,
            tx.SpecialDiscount,
            tx.Total,
            [
                .. tx.Items.Select(i => new TransactionItemDto(
                    i.ItemType,
                    i.CatalogId,
                    i.CatalogName,
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice
                )),
            ],
            tx.IsVoided,
            tx.VoidedBy,
            tx.VoidedAt,
            tx.VoidReason
        );
    }

    public async Task<List<TransactionSummaryDto>> FilterAsync(
        TransactionFilterDto filter,
        CancellationToken ct
    )
    {
        var query = db.Transactions.AsQueryable();

        if (filter.TodayOnly)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(t => t.CreatedAt >= today && t.CreatedAt < today.AddDays(1));
        }

        if (filter.From is not null && filter.To is not null)
        {
            query = query.Where(t => t.CreatedAt >= filter.From && t.CreatedAt <= filter.To);
        }
        else if (
            (filter.From is not null && filter.To is null)
            || (filter.From is null && filter.To is not null)
        )
        {
            throw new InvalidOperationException("Both 'From' and 'To' must be provided together.");
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransactionSummaryDto(t.Id, t.CreatedAt, t.Total, t.Items.Count))
            .ToListAsync(ct);
    }

    private async Task<CatalogRowType> InferItemTypeAsync(Guid id, CancellationToken ct)
    {
        var isBatch = await db.InventoryBatches.AnyAsync(b => b.Id == id, ct);
        if (isBatch)
            return CatalogRowType.Product;

        var isBundle = await db.Bundles.AnyAsync(b => b.Id == id, ct);
        if (isBundle)
            return CatalogRowType.Bundle;

        throw new InvalidOperationException($"Unknown catalog ID: {id}");
    }

    private async Task ProcessProduct(
        Transaction tx,
        TransactionItemCreateDto item,
        CancellationToken ct
    )
    {
        var batch =
            await db
                .InventoryBatches.Include(b => b.Product)
                .ThenInclude(p => p.Consumable)
                .FirstOrDefaultAsync(b => b.Id == item.CatalogId, ct)
            ?? throw new InvalidOperationException("Product batch not found.");

        var product = batch.Product;
        var qty = item.Quantity;

        if (product.IsConsumable && product.Consumable?.UsesMax > 0)
        {
            if (product.Consumable.UsesLeft < qty)
                throw new InvalidOperationException("Not enough uses left.");

            product.Consumable.UsesLeft -= qty;
        }
        else
        {
            if (batch.QuantityOnHand < qty)
                throw new InvalidOperationException("Not enough quantity on hand.");

            batch.QuantityOnHand -= qty;
        }

        tx.Items.Add(
            new TransactionItem
            {
                Transaction = tx,
                CatalogId = batch.Id,
                ItemType = CatalogRowType.Product,
                CatalogName = product.Brand,
                Quantity = qty,
                UnitPrice = product.RetailPrice,
                TotalPrice = qty * product.RetailPrice,
            }
        );
    }

    private async Task ProcessBundle(
        Transaction tx,
        TransactionItemCreateDto item,
        CancellationToken ct
    )
    {
        var bundle =
            await db
                .Bundles.Include(b => b.BundleItems)
                .ThenInclude(i => i.InventoryBatch)
                .Include(b => b.BundleItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Consumable)
                .FirstOrDefaultAsync(b => b.Id == item.CatalogId, ct)
            ?? throw new InvalidOperationException("Bundle not found.");

        var qty = item.Quantity;

        foreach (var bi in bundle.BundleItems)
        {
            var totalQty = qty * (bi.Quantity == 0 ? bi.Uses : bi.Quantity);

            if (bi.Product.IsConsumable && bi.Uses > 0)
            {
                var usesLeft = bi.Product.Consumable?.UsesLeft ?? 0;
                if (usesLeft < totalQty)
                    throw new InvalidOperationException(
                        $"Not enough uses left for '{bi.Product.Brand}'"
                    );

                bi.Product.Consumable!.UsesLeft -= totalQty;
            }
            else
            {
                if (bi.InventoryBatch.QuantityOnHand < totalQty)
                    throw new InvalidOperationException(
                        $"Not enough stock for '{bi.Product.Brand}'"
                    );

                bi.InventoryBatch.QuantityOnHand -= totalQty;
            }
        }

        tx.Items.Add(
            new TransactionItem
            {
                Transaction = tx,
                CatalogId = bundle.Id,
                ItemType = CatalogRowType.Bundle,
                CatalogName = bundle.Name,
                Quantity = qty,
                UnitPrice = bundle.Price,
                TotalPrice = qty * bundle.Price,
            }
        );
    }
}
