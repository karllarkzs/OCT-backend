using PharmaBack.WebApi.DTO.Product;

namespace PharmaBack.WebApi.Services.Products;

public interface IProductService
{
    Task<IReadOnlyList<Guid>> BatchCreateOrUpdateAsync(
        IEnumerable<CreateProductDto> dtos,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<GetProductDto>> GetAllAsync(CancellationToken ct = default);
}
