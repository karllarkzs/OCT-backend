using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.DTO.Restock;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<Guid> RestockAsync(
    CreateRestockDto dto,
    string userId,
    string userName,
    CancellationToken ct = default)
    {
        var restock = new ProductRestock
        {
            Id = Guid.NewGuid(),
            ReceivedDate = dto.ReceivedDate,
            ReceivedBy = dto.ReceivedBy,
            SupplierName = dto.SupplierName,
            ReferenceNumber = dto.ReferenceNumber,
            CreatedByUserId = userId,
            CreatedByUserName = userName,
            CreatedAt = DateTime.UtcNow
        };

        decimal totalCost = 0;

        foreach (var item in dto.Items)
        {
            Product? product = await db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);

            if (product is not null)
            {
                product.Quantity += item.Quantity;
                product.RetailPrice = item.RetailPrice;
                product.WholesalePrice = item.PurchasePrice;

                db.Entry(product).State = EntityState.Modified;

                await audit.LogChangeAsync(
                    product,
                    ProductActionType.Restocked,
                    new[] { nameof(product.Quantity), nameof(product.RetailPrice), nameof(product.WholesalePrice) },
                    userId,
                    userName,
                    ct
                );
            }
            else
            {

                throw new InvalidOperationException($"Product {item.ProductId} not found.");
            }

            var restockItem = new ProductRestockItem
            {
                ProductRestockId = restock.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                PurchasePrice = item.PurchasePrice,
                RetailPrice = item.RetailPrice
            };

            restock.Items.Add(restockItem);
            totalCost += item.Quantity * item.PurchasePrice;
        }

        restock.TotalCost = totalCost;

        db.ProductRestocks.Add(restock);
        await db.SaveChangesAsync(ct);

        return restock.Id;
    }


}
