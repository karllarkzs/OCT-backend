using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
   public async Task SoftDeleteAsync(
    IEnumerable<Guid> ids,
    string userId,
    string userName,
    CancellationToken ct = default
)
{
    var products = await db.Products
        .Where(p => ids.Contains(p.Id))
        .ToListAsync(ct);

    var foundIds = products.Select(p => p.Id).ToHashSet();
    var missingIds = ids.Where(id => !foundIds.Contains(id)).ToList();

    if (missingIds.Count > 0)
        throw new InvalidOperationException($"Products not found: {string.Join(", ", missingIds)}");

    foreach (var product in products)
    {
        if (!product.IsDeleted)
        {
            product.IsDeleted = true;
            db.Entry(product).Property(p => p.IsDeleted).IsModified = true;

            await audit.LogChangeAsync(
                product,
                ProductActionType.Deleted,
                [nameof(product.IsDeleted)],
                userId,
                userName,
                ct
            );
        }
    }

    await db.SaveChangesAsync(ct);
}

}
