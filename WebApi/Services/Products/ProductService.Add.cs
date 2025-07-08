namespace PharmaBack.WebApi.Services.Products;

using System.Threading;
using PharmaBack.DTO.Product;

public sealed partial class ProductService
{
    public Task<Guid> AddOrUpdateAsync(AddStockRequestDto dto, CancellationToken ct = default) =>
        Process(dto, true, ct);
}
