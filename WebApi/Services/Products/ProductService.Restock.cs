using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<Guid> RestockAsync(RestockProductDto dto, CancellationToken ct = default)
    {
        Product? product = null;

        if (dto.Id.HasValue)
        {
            product = await db.Products.FirstOrDefaultAsync(p => p.Id == dto.Id.Value, ct);

            if (product is not null && product.ExpiryDate == dto.ExpiryDate)
            {
                product.Quantity += dto.Quantity;
                db.Entry(product).Property(p => p.Quantity).IsModified = true;

                await db.SaveChangesAsync(ct);
                return product.Id;
            }
        }

        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Barcode = dto.Barcode,
            Brand = dto.Brand,
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
        };

        db.Products.Add(newProduct);
        await db.SaveChangesAsync(ct);

        return newProduct.Id;
    }
}
