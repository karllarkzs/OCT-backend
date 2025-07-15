using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.DTO.Transactions;
using PharmaBack.WebApi.Models;
using PharmaBack.WebApi.Services.Products;

namespace PharmaBack.WebApi.Services.Transactions;

public sealed class TransactionService(PharmaDbContext db, IProductAuditService audit)
    : ITransactionService
{
    private static readonly TimeZoneInfo ManilaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
        "Singapore"
    );

    public async Task<Guid> ProcessAsync(TransactionCreateDto dto, CancellationToken ct)
    {
        if (dto.ModeOfPayment == "cash")
        {
            if (dto.CashInHand == null)
                throw new InvalidOperationException("CashInHand is required for cash payments.");
            if (dto.Change == null)
                throw new InvalidOperationException("Change is required for cash payments.");
        }
        else if (dto.ModeOfPayment == "gcash")
        {
            if (string.IsNullOrWhiteSpace(dto.ReferenceNumber))
                throw new InvalidOperationException(
                    "ReferenceNumber is required for GCash payments."
                );
        }
        else
        {
            throw new InvalidOperationException("Unsupported mode of payment.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var utcNow = DateTime.UtcNow;
        var receiptId = TimeZoneInfo
            .ConvertTimeFromUtc(utcNow, ManilaTimeZone)
            .ToString("yyMMddHHmmssfff");

        var tx = new Transaction
        {
            CreatedAt = utcNow,
            ReceiptId = receiptId,
            Subtotal = dto.Subtotal,
            Vat = dto.Vat,
            SpecialDiscount = dto.SpecialDiscount,
            Total = dto.Total,
            DiscountId = dto.DiscountId,
            CashInHand = dto.CashInHand,
            Change = dto.Change,
            ModeOfPayment = dto.ModeOfPayment,
            ReferenceNumber = dto.ReferenceNumber,
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

                case CatalogRowType.Package:
                    await ProcessPackage(tx, item, ct);
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

        var products = await db
            .Products.Where(p => tx.Items.Select(i => i.CatalogId).Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var Packages = await db
            .Packages.Where(b => tx.Items.Select(i => i.CatalogId).Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, ct);

        var items = new List<TransactionItemDto>();

        foreach (var item in tx.Items)
        {
            string catalogName;

            if (
                item.ItemType == CatalogRowType.Product
                && products.TryGetValue(item.CatalogId, out var product)
            )
            {
                var generic = string.IsNullOrWhiteSpace(product.Generic) ? "" : product.Generic;
                var formulation = string.IsNullOrWhiteSpace(product.Formulation)
                    ? ""
                    : product.Formulation;
                var inner =
                    $"{generic}{(string.IsNullOrWhiteSpace(formulation) ? "" : $" - {formulation}")}".Trim();

                catalogName = string.IsNullOrWhiteSpace(inner)
                    ? product.Brand
                    : $"{product.Brand} ({inner})";
            }
            else if (
                item.ItemType == CatalogRowType.Package
                && Packages.TryGetValue(item.CatalogId, out var Package)
            )
            {
                catalogName = Package.Name;
            }
            else
            {
                catalogName = item.CatalogName;
            }

            items.Add(
                new TransactionItemDto(
                    item.ItemType,
                    item.CatalogId,
                    catalogName,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice
                )
            );
        }

        return new TransactionDetailDto(
            tx.Id,
            tx.ReceiptId!,
            TimeZoneInfo.ConvertTimeFromUtc(tx.CreatedAt, ManilaTimeZone),
            tx.Subtotal,
            tx.Vat,
            tx.SpecialDiscount,
            tx.Total,
            items,
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
            var manilaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ManilaTimeZone);
            var manilaToday = manilaNow.Date;
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(manilaToday, ManilaTimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(manilaToday.AddDays(1), ManilaTimeZone);

            query = query.Where(t => t.CreatedAt >= startUtc && t.CreatedAt < endUtc);
        }

        if (filter.From is not null && filter.To is not null)
        {
            query = query.Where(t => t.CreatedAt >= filter.From && t.CreatedAt <= filter.To);
        }
        else if (filter.From is not null || filter.To is not null)
        {
            throw new InvalidOperationException("Both 'From' and 'To' must be provided together.");
        }

        var results = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.ReceiptId,
                t.CreatedAt,
                t.Total,
                ItemCount = t.Items.Count,
                t.IsVoided,
            })
            .ToListAsync(ct);

        return
        [
            .. results.Select(t => new TransactionSummaryDto(
                t.Id,
                t.ReceiptId!,
                TimeZoneInfo.ConvertTimeFromUtc(t.CreatedAt, ManilaTimeZone),
                t.Total,
                t.ItemCount,
                t.IsVoided
            )),
        ];
    }

    private async Task<CatalogRowType> InferItemTypeAsync(Guid id, CancellationToken ct)
    {
        var isProduct = await db.Products.AnyAsync(p => p.Id == id, ct);
        if (isProduct)
            return CatalogRowType.Product;

        var isPackage = await db.Packages.AnyAsync(b => b.Id == id, ct);
        if (isPackage)
            return CatalogRowType.Package;

        throw new InvalidOperationException($"Unknown catalog ID: {id}");
    }

    private async Task ProcessProduct(
        Transaction tx,
        TransactionItemCreateDto item,
        CancellationToken ct
    )
    {
        var product =
            await db.Products.FirstOrDefaultAsync(p => p.Id == item.CatalogId, ct)
            ?? throw new InvalidOperationException("Product not found.");

        if (product.Quantity < item.Quantity)
            throw new InvalidOperationException("Not enough quantity on hand.");

        product.Quantity -= item.Quantity;

        tx.Items.Add(
            new TransactionItem
            {
                Transaction = tx,
                CatalogId = product.Id,
                ItemType = CatalogRowType.Product,
                CatalogName = product.Brand,
                Quantity = item.Quantity,
                UnitPrice = product.RetailPrice,
                TotalPrice = item.Quantity * product.RetailPrice,
            }
        );
    }

    private async Task ProcessPackage(
        Transaction tx,
        TransactionItemCreateDto item,
        CancellationToken ct
    )
    {
        var Package =
            await db
                .Packages.Include(b => b.PackageItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.Id == item.CatalogId, ct)
            ?? throw new InvalidOperationException("Package not found.");

        foreach (var bi in Package.PackageItems)
        {
            var requiredQty = item.Quantity * bi.Quantity;
            if (bi.Product.Quantity < requiredQty)
                throw new InvalidOperationException($"Not enough stock for '{bi.Product.Brand}'");

            bi.Product.Quantity -= requiredQty;
        }

        tx.Items.Add(
            new TransactionItem
            {
                Transaction = tx,
                CatalogId = Package.Id,
                ItemType = CatalogRowType.Package,
                CatalogName = Package.Name,
                Quantity = item.Quantity,
                UnitPrice = Package.Price,
                TotalPrice = item.Quantity * Package.Price,
            }
        );
    }

    public async Task VoidTransactionAsync(
        TransactionVoidDto dto,
        string? username,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new UnauthorizedAccessException("Could not determine user identity.");

        var transaction =
            await db
                .Transactions.Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == dto.TransactionId, ct)
            ?? throw new InvalidOperationException("Transaction not found.");
        if (transaction.IsVoided)
            throw new InvalidOperationException("Transaction is already voided.");

        transaction.IsVoided = true;
        transaction.VoidedBy = username;
        transaction.VoidedAt = DateTime.UtcNow;
        transaction.VoidReason = dto.Reason;

        foreach (var item in transaction.Items)
        {
            switch (item.ItemType)
            {
                case CatalogRowType.Product:
                {
                    var product = await db.Products.FirstOrDefaultAsync(
                        p => p.Id == item.CatalogId,
                        ct
                    );
                    if (product is not null)
                        product.Quantity += item.Quantity;
                    break;
                }

                case CatalogRowType.Package:
                {
                    var Package = await db
                        .Packages.Include(b => b.PackageItems)
                        .ThenInclude(bi => bi.Product)
                        .FirstOrDefaultAsync(b => b.Id == item.CatalogId, ct);

                    if (Package is null)
                        continue;

                    foreach (var PackageItem in Package.PackageItems)
                    {
                        var restockQty = PackageItem.Quantity * item.Quantity;
                        if (PackageItem.Product is not null)
                            PackageItem.Product.Quantity += restockQty;
                    }

                    break;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
