using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed class ProductAuditService(PharmaDbContext db) : IProductAuditService
{
    public async Task LogChangeAsync(
        Product product,
        ProductActionType actionType,
        IEnumerable<string> changedFields,
        string userId,
        string userName,
        CancellationToken ct = default
    )
    {
        var history = new ProductHistory
        {
            ProductId = product.Id,
            Barcode = product.Barcode,
            Brand = product.Brand,
            Generic = product.Generic,
            RetailPrice = product.RetailPrice,
            WholesalePrice = product.WholesalePrice,
            Quantity = product.Quantity,
            ExpiryDate = product.ExpiryDate,
            ReceivedDate = product.ReceivedDate,
            Location = product.Location,
            MinStock = product.MinStock,
            IsDeleted = product.IsDeleted,
            IsDiscountable = product.IsDiscountable,
            Category = product.Category,
            Formulation = product.Formulation,
            Company = product.Company,
            Type = product.Type,
            ChangedAt = DateTime.UtcNow,
            ActionType = actionType,
            ChangedByUserId = userId,
            ChangedByUserName = userName,
            Changes = [.. changedFields.Select(f => new ProductHistoryChange { FieldName = f })],
        };

        db.ProductHistories.Add(history);
        await db.SaveChangesAsync(ct);
    }
}
