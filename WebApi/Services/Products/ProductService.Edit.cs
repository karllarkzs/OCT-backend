using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.DTO.Product;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<Guid> EditProductAsync(EditProductDto dto, CancellationToken ct = default)
    {
        var product =
            await db.Products.FirstOrDefaultAsync(p => p.Id == dto.Id, ct)
            ?? throw new InvalidOperationException("Product not found.");

        product.Barcode = dto.Barcode ?? product.Barcode;
        product.Brand = dto.Brand ?? product.Brand;
        product.Generic = dto.Generic;
        product.RetailPrice = dto.RetailPrice;
        product.WholesalePrice = dto.WholesalePrice;
        product.Quantity = dto.Quantity;
        product.ExpiryDate = dto.ExpiryDate;
        product.ReceivedDate = dto.ReceivedDate;
        product.Location = dto.Location;
        product.MinStock = dto.MinStock;
        product.Category = dto.Category;
        product.Formulation = dto.Formulation;
        product.Company = dto.Company;
        product.Type = dto.Type;

        await db.SaveChangesAsync(ct);
        return product.Id;
    }
}
