using PharmaBack.WebApi.DTO.Product;

namespace PharmaBack.WebApi.Services.Products;

public interface IProductService
{
    Task<IReadOnlyList<Guid>> BatchCreateOrUpdateAsync(
        IEnumerable<CreateProductDto> dtos,
        string userId,
        string userName,
        CancellationToken ct = default
    );

    Task<IReadOnlyList<Guid>> EditProductAsync(
        IEnumerable<EditProductDto> dtos,
        string userId,
        string userName,
        CancellationToken ct = default
    );

    Task<IReadOnlyList<GetProductDto>> GetAllAsync(
        bool includeHistory = false,
        CancellationToken ct = default
    );
    Task<Guid> RestockAsync(RestockProductDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetAllLocationsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetAllCompaniesAsync(CancellationToken ct = default);
Task SoftDeleteAsync(IEnumerable<Guid> ids, string userId, string userName, CancellationToken ct = default);
}
