namespace PharmaBack.Services.Brands;

using PharmaBack.DTO;

public interface IBrandService
{
    Task<List<BrandDto>> GetAllAsync(CancellationToken ct);
    Task<BrandDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<BrandDto> CreateAsync(UpsertBrandDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(Guid id, UpsertBrandDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
