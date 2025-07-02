namespace PharmaBack.DTO;

public sealed record CatalogRowDto(
    Guid Id,
    CatalogRowType ItemType,
    string Barcode,
    string? Name,
    string? Generic,
    string? Brand,
    string? Category,
    string? Formulation,
    string? Company,
    DateOnly? Expiry,
    int Quantity,
    string? Location,
    decimal? Price
);

public enum CatalogRowType
{
    Product,
    Bundle,
}
