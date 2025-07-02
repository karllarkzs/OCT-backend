namespace PharmaBack.Services.Products;

using Microsoft.EntityFrameworkCore;
using PharmaBack.DTO.Product;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<GetProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var q = db
            .InventoryBatches.AsNoTracking()
            .Where(b => b.QuantityOnHand > 0 && !b.Product.IsDeleted)
            .OrderBy(b => b.Product.Brand)
            .ThenBy(b => b.ExpiryDate ?? DateOnly.MaxValue)
            .Select(b => new GetProductDto(
                b.ProductId,
                b.Id,
                b.Product.Barcode,
                b.Product.Generic,
                b.Product.Brand,
                b.Product.Category ?? null,
                b.Product.Formulation ?? null,
                b.Product.Company ?? null,
                b.Product.RetailPrice,
                b.Product.WholesalePrice,
                b.QuantityOnHand,
                b.ExpiryDate,
                b.Location != null ? b.Location.Name : null,
                b.LocationId,
                b.Product.IsConsumable,
                b.Product.Consumable != null ? (int?)b.Product.Consumable.UsesMax : null,
                b.Product.Consumable != null ? (int?)b.Product.Consumable.UsesLeft : null
            ));

        return await q.ToListAsync(ct);
    }
}
