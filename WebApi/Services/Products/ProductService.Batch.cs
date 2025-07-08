namespace PharmaBack.WebApi.Services.Products;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmaBack.DTO.Product;

public sealed partial class ProductService
{
    public async Task<IReadOnlyList<Guid>> BatchAddOrUpdateAsync(
        IEnumerable<AddStockRequestDto> dtos,
        CancellationToken ct = default
    )
    {
        var ids = new List<Guid>();
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        foreach (var d in dtos)
            ids.Add(await Process(d, false, ct));

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return ids;
    }
}
