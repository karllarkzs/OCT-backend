namespace PharmaBack.WebApi.Services.Products;

using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<GetProductDto>> GetAllAsync(
        bool includeHistory = false,
        CancellationToken ct = default
    )
    {
        var query = db.Products.AsNoTracking().Where(p => !p.IsDeleted && p.Quantity > 0);

        if (includeHistory)
        {
            query = query.Include(p => p.History).ThenInclude(h => h.Changes);
        }

        var products = await query
            .OrderBy(p => p.Brand)
            .ThenBy(p => p.ExpiryDate ?? DateOnly.MaxValue)
            .ToListAsync(ct);

        return
        [
            .. products.Select(p => new GetProductDto(
                Id: p.Id,
                Barcode: p.Barcode,
                Generic: p.Generic,
                Brand: p.Brand,
                Category: p.Category,
                Formulation: p.Formulation,
                Company: p.Company,
                Type: p.Type,
                RetailPrice: p.RetailPrice,
                WholesalePrice: p.WholesalePrice,
                Quantity: p.Quantity,
                MinStock: p.MinStock,
                ExpiryDate: p.ExpiryDate,
                Location: p.Location,
                ReceivedDate: p.ReceivedDate,
                IsExpired: p.IsExpired,
                IsLowStock: p.IsLowStock,
                IsDiscountable: p.IsDiscountable,
                History: includeHistory
                    ? p
                        .History.OrderByDescending(h => h.ChangedAt)
                        .Select(h => new ProductSnapshotDto(
                            Id: h.Id,
                            ProductId: h.ProductId,
                            Barcode: h.Barcode,
                            Brand: h.Brand,
                            Generic: h.Generic,
                            RetailPrice: h.RetailPrice,
                            WholesalePrice: h.WholesalePrice,
                            Quantity: h.Quantity,
                            ExpiryDate: h.ExpiryDate,
                            ReceivedDate: h.ReceivedDate,
                            Location: h.Location,
                            MinStock: h.MinStock,
                            IsDeleted: h.IsDeleted,
                            IsDiscountable: h.IsDiscountable,
                            Category: h.Category,
                            Formulation: h.Formulation,
                            Company: h.Company,
                            Type: h.Type,
                            ChangedAt: h.ChangedAt,
                            ActionType: h.ActionType,
                            ChangedByUserId: h.ChangedByUserId,
                            ChangedByUserName: h.ChangedByUserName,
                            Changes: [.. h.Changes.Select(c => c.FieldName)]
                        ))
                        .ToList()
                    : []
            )),
        ];
    }
}
