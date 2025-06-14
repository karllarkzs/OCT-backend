namespace PharmaBack.Services.Products;

using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.Models;

public sealed class ProductService(PharmaDbContext db) : IProductService
{
    public async Task<Guid> ProcessOneAsync(AddStockRequest dto, CancellationToken ct = default)
    {
        var product = await db
            .Products.Include(p => p.Batches)
            .FirstOrDefaultAsync(p => p.Barcode == dto.Barcode, ct);

        if (product is null)
        {
            product = new Product
            {
                Barcode = dto.Barcode,
                Name = dto.Name,
                RetailPrice = dto.RetailPrice,
                WholesalePrice = dto.WholesalePrice,
                HasExpiry = dto.HasExpiry,
                IsConsumable = dto.IsConsumable,
            };
            db.Products.Add(product);

            if (dto.IsConsumable)
                product.Consumable = new ConsumableExtension { UsesMax = 1, UsesLeft = 1 };
        }
        else
        {
            product.RetailPrice = dto.RetailPrice;
            product.WholesalePrice = dto.WholesalePrice;
            product.HasExpiry = dto.HasExpiry;
            product.IsConsumable = dto.IsConsumable;
        }

        InventoryBatch? batch;

        if (dto.HasExpiry)
        {
            if (dto.ExpiryDate is null)
                throw new ArgumentException("Expiry date required when HasExpiry = true");

            batch = product.Batches.FirstOrDefault(b => b.ExpiryDate == dto.ExpiryDate.Value.Date);

            if (batch is null)
            {
                batch = new InventoryBatch
                {
                    Product = product,
                    ExpiryDate = dto.ExpiryDate.Value.Date,
                    QuantityOnHand = dto.Quantity,
                };
                db.InventoryBatches.Add(batch);
            }
            else
            {
                batch.QuantityOnHand += dto.Quantity;
            }
        }
        else
        {
            batch = product.Batches.FirstOrDefault(b => b.ExpiryDate == null);

            if (batch is null)
            {
                batch = new InventoryBatch
                {
                    Product = product,
                    ExpiryDate = null,
                    QuantityOnHand = dto.Quantity,
                };
                db.InventoryBatches.Add(batch);
            }
            else
            {
                batch.QuantityOnHand += dto.Quantity;
            }
        }

        await db.SaveChangesAsync(ct);
        return product.ProductId;
    }

    public async Task<Guid> AddOrUpdateAsync(AddStockRequest dto, CancellationToken ct = default) =>
        await ProcessOneAsync(dto, ct);

    public async Task<IReadOnlyList<Guid>> BatchAddOrUpdateAsync(
        IEnumerable<AddStockRequest> dtos,
        CancellationToken ct = default
    )
    {
        var ids = new List<Guid>();

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        foreach (var dto in dtos)
            ids.Add(await ProcessOneAsync(dto, ct));

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return ids;
    }
}
