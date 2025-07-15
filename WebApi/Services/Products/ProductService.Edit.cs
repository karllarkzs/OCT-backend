using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<Guid>> EditProductAsync(
        IEnumerable<EditProductDto> dtos,
        string userId,
        string userName,
        CancellationToken ct = default
    )
    {
        var results = new List<Guid>();

        foreach (var dto in dtos)
        {
            var product =
                await db.Products.FirstOrDefaultAsync(p => p.Id == dto.Id, ct)
                ?? throw new InvalidOperationException($"Product {dto.Id} not found.");

            var changedFields = new List<string>();

            void Track<T>(T oldVal, T newVal, string name)
            {
                if (!EqualityComparer<T>.Default.Equals(oldVal, newVal))
                {
                    changedFields.Add(name);
                    // assign after tracking:
                    typeof(Product).GetProperty(name)!.SetValue(product, newVal);
                }
            }

            Track(product.Barcode, dto.Barcode ?? product.Barcode, nameof(product.Barcode));
            Track(product.Brand, dto.Brand ?? product.Brand, nameof(product.Brand));
            Track(product.Generic, dto.Generic, nameof(product.Generic));
            Track(product.RetailPrice, dto.RetailPrice, nameof(product.RetailPrice));
            Track(product.WholesalePrice, dto.WholesalePrice, nameof(product.WholesalePrice));
            Track(product.Quantity, dto.Quantity, nameof(product.Quantity));
            Track(product.ExpiryDate, dto.ExpiryDate, nameof(product.ExpiryDate));
            Track(product.ReceivedDate, dto.ReceivedDate, nameof(product.ReceivedDate));
            Track(product.Location, dto.Location, nameof(product.Location));
            Track(product.MinStock, dto.MinStock, nameof(product.MinStock));
            Track(product.Category, dto.Category, nameof(product.Category));
            Track(product.Formulation, dto.Formulation, nameof(product.Formulation));
            Track(product.Company, dto.Company, nameof(product.Company));
            Track(product.Type, dto.Type, nameof(product.Type));
            Track(product.IsDiscountable, dto.IsDiscountable, nameof(product.IsDiscountable));

            if (changedFields.Count > 0)
            {
                await audit.LogChangeAsync(
                    product,
                    ProductActionType.Updated,
                    changedFields,
                    userId,
                    userName,
                    ct
                );
            }

            results.Add(product.Id);
        }

        await db.SaveChangesAsync(ct);
        return results;
    }
}
