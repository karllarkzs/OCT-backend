using PharmaBack.DTO;
using PharmaBack.DTO.Product;

namespace PharmaBack.Services.Products;

public interface IProductService
{
    Task<Guid> AddOrUpdateAsync(AddStockRequestDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<Guid>> BatchAddOrUpdateAsync(
        IEnumerable<AddStockRequestDto> dtos,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<GetProductDto>> GetAsync(string? search, CancellationToken ct = default);
    Task<IReadOnlyList<GetProductDto>> GetAllAsync(CancellationToken ct = default);
}
