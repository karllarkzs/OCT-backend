using PharmaBack.DTO;

namespace PharmaBack.Services.Bundles;

public interface IBundleService
{
    Task<List<BundleDto>> GetAllAsync(CancellationToken ct);
    Task<BundleDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<BundleDto> CreateAsync(BundleDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(Guid id, BundleDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<bool> AddProductAsync(Guid bundleId, BundleItemDto item, CancellationToken ct);
    Task<bool> RemoveProductAsync(Guid bundleId, Guid productId, CancellationToken ct);
}
