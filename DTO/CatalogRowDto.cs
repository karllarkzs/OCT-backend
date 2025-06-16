namespace PharmaBack.DTO;

public sealed record CatalogRowDto(
    Guid Id,
    CatalogRowType RowType,
    string Code,
    string Name,
    DateTime? Expiry,
    int Quantity,
    string? Location,
    decimal? Price
);

public enum CatalogRowType
{
    Product,
    Bundle,
}
