using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<Guid>> BatchCreateOrUpdateAsync(
        IEnumerable<CreateProductDto> dtos,
        string userId,
        string userName,
        CancellationToken ct = default
    )
    {
        var results = new List<Guid>();

        foreach (var dto in dtos)
        {
            var product = await db.Products.FirstOrDefaultAsync(
                p =>
                    p.Barcode == dto.Barcode
                    && p.Brand == dto.Brand
                    && p.Generic == dto.Generic
                    && p.RetailPrice == dto.RetailPrice
                    && p.WholesalePrice == dto.WholesalePrice
                    && p.ExpiryDate == dto.ExpiryDate
                    && p.ReceivedDate == dto.ReceivedDate
                    && p.Location == dto.Location
                    && p.MinStock == dto.MinStock
                    && p.Category == dto.Category
                    && p.Formulation == dto.Formulation
                    && p.Company == dto.Company
                    && p.Type == dto.Type
                    && p.IsDiscountable == dto.IsDiscountable
                    && p.IsReagent == dto.IsReagent,
                ct
            );

            if (product is not null)
            {
                product.Quantity += dto.Quantity;
                db.Entry(product).Property(p => p.Quantity).IsModified = true;

                await audit.LogChangeAsync(
                    product,
                    ProductActionType.Restocked,
                    ["Quantity"],
                    userId,
                    userName,
                    ct
                );
            }
            else
            {
                product = new Product
                {
                    Id = Guid.NewGuid(),
                    Barcode = dto.Barcode ?? "",
                    Brand = dto.Brand ?? "",
                    Generic = dto.Generic,
                    RetailPrice = dto.RetailPrice,
                    WholesalePrice = dto.WholesalePrice,
                    Quantity = dto.Quantity,
                    ExpiryDate = dto.ExpiryDate,
                    ReceivedDate = dto.ReceivedDate,
                    Location = dto.Location,
                    MinStock = dto.MinStock,
                    Category = dto.Category,
                    Formulation = dto.Formulation,
                    Company = dto.Company,
                    Type = dto.Type,
                    IsDiscountable = dto.IsDiscountable,
                    IsReagent = dto.IsReagent,
                };

                db.Products.Add(product);

                await audit.LogChangeAsync(
                    product,
                    ProductActionType.Added,
                    ["All"],
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
