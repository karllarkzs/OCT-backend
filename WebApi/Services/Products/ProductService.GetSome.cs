using Microsoft.EntityFrameworkCore;

namespace PharmaBack.WebApi.Services.Products;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<string>> GetAllLocationsAsync(CancellationToken ct = default)
    {
        return await db
            .Products.AsNoTracking()
            .Where(p => !string.IsNullOrWhiteSpace(p.Location))
            .Select(p => p.Location!)
            .Distinct()
            .OrderBy(loc => loc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetAllCompaniesAsync(CancellationToken ct = default)
    {
        return await db
            .Products.AsNoTracking()
            .Where(p => !string.IsNullOrWhiteSpace(p.Company))
            .Select(p => p.Company!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);
    }
}
