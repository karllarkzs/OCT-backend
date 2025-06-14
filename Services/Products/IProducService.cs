namespace PharmaBack.Services.Products;

public interface IProductService
{
    Task<Guid> ProcessOneAsync(AddStockRequest dto, CancellationToken ct = default);

    Task<IReadOnlyList<Guid>> BatchAddOrUpdateAsync(
        IEnumerable<AddStockRequest> dtos,
        CancellationToken ct = default
    );
}
