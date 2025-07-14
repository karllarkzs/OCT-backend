namespace PharmaBack.WebApi.Services.Products;

using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<GetProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await db
            .Products.AsNoTracking()
            .Where(p => !p.IsDeleted && p.Quantity > 0)
            .OrderBy(p => p.Brand)
            .ThenBy(p => p.ExpiryDate ?? DateOnly.MaxValue)
            .Select(p => new GetProductDto(
                p.Id,
                p.Barcode,
                p.Generic,
                p.Brand,
                p.Category,
                p.Formulation,
                p.Company,
                p.Type,
                p.RetailPrice,
                p.WholesalePrice,
                p.Quantity,
                p.MinStock,
                p.ExpiryDate,
                p.Location,
                p.ReceivedDate,
                p.IsExpired,
                p.IsLowStock
            ))
            .ToListAsync(ct);

        return products;
    }
}
