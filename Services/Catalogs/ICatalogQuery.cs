namespace PharmaBack.Services.Catalogs;

using PharmaBack.DTO;

public interface ICatalogQuery
{
    Task<IReadOnlyList<CatalogRowDto>> GetAsync(string? search, CancellationToken ct);
}
