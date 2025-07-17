using System.Collections.Generic;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.DTO.Restock;

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
    Task<Guid> RestockAsync(
    CreateRestockDto dto,
    string userId,
    string userName,
    CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetAllLocationsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task SoftDeleteAsync(IEnumerable<Guid> ids, string userId, string userName, CancellationToken ct = default);
}
