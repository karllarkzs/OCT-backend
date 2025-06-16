namespace PharmaBack.Services.Catalog;

using PharmaBack.DTO;

public interface ICatalogQuery
{
    Task<IReadOnlyList<CatalogRowDto>> GetAsync(string? search, CancellationToken ct);
}
