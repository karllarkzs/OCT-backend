namespace PharmaBack.WebApi.Services.Products;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmaBack.DTO.Product;
using PharmaBack.Helpers;
using PharmaBack.WebApi.Models;

public sealed partial class ProductService
{
    async Task<Guid> Process(AddStockRequestDto dto, bool save, CancellationToken ct)
    {
        var effectiveBarcode = string.IsNullOrWhiteSpace(dto.Barcode)
            ? BarcodeGenerator.GenerateInternalEan13()
            : dto.Barcode;
        var product = await db
            .Products.Include(p => p.Batches)
            .Include(p => p.Consumable)
            .FirstOrDefaultAsync(p => p.Barcode == effectiveBarcode || p.Id == dto.Id, ct);

        if (product is null)
        {
            product = new Product
            {
                Barcode = effectiveBarcode,
                Generic = dto.Generic,
                RetailPrice = dto.RetailPrice,
                WholesalePrice = dto.WholesalePrice,
                HasExpiry = dto.HasExpiry,
                IsConsumable = dto.IsConsumable,
                Stock = 0,
                Brand = dto.Brand ?? "",
                Category = dto.Category,
                Formulation = dto.Formulation,
                Company = dto.Company,
            };
            if (dto.IsConsumable)
                product.Consumable = new ConsumableExtension
                {
                    UsesMax = (int)dto.UsesMax!,
                    UsesLeft = (int)dto.UsesLeft!,
                };
            db.Products.Add(product);
        }
        else
        {
            product.Brand = dto.Brand ?? "";
            product.RetailPrice = dto.RetailPrice;
            product.WholesalePrice = dto.WholesalePrice;
            product.HasExpiry = dto.HasExpiry;
            if (dto.Brand != null)
                product.Brand = dto.Brand;
            if (dto.Category != null)
                product.Category = dto.Category;
            if (dto.Formulation != null)
                product.Formulation = dto.Formulation;
            if (dto.Company != null)
                product.Company = dto.Company;
            if (!product.IsConsumable && dto.IsConsumable)
                product.Consumable = new ConsumableExtension { UsesMax = 1, UsesLeft = 1 };
        }

        if (dto.Quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(dto),
                dto.Quantity,
                "Quantity must be positive"
            );

        var expiry = dto.HasExpiry ? dto.ExpiryDate : null;
        if (dto.HasExpiry && expiry is null)
            throw new ArgumentException("Expiry date required when HasExpiry = true", nameof(dto));

        var batch = product.Batches.FirstOrDefault(b =>
            b.ExpiryDate == expiry && b.LocationId == dto.LocationId
        );

        if (batch is null)
        {
            batch = new InventoryBatch
            {
                Product = product,
                ExpiryDate = expiry,
                QuantityOnHand = dto.Quantity,
                LocationId = dto.LocationId,
            };
            db.InventoryBatches.Add(batch);
        }
        else
        {
            batch.QuantityOnHand += dto.Quantity;
            db.Entry(batch).Property(b => b.QuantityOnHand).IsModified = true;
        }

        product.Stock = product.Batches.Sum(b => b.QuantityOnHand);
        db.Entry(product).Property(p => p.Stock).IsModified = true;

        if (save)
            await db.SaveChangesAsync(ct);
        return product.Id;
    }
}
