namespace PharmaBack.WebApi.Services.Catalogs;

using PharmaBack.WebApi.DTO;

public interface ICatalogQuery
{
    Task<IReadOnlyList<CatalogRowDto>> GetAsync(string? search, CancellationToken ct);
}
