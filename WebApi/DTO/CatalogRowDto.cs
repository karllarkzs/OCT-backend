namespace PharmaBack.WebApi.DTO;

public sealed record CatalogRowDto(
    Guid Id,
    CatalogRowType ItemType,
    string Barcode,
    string? Name,
    string? Generic,
    string? Brand,
    string? Category,
    string? Type,
    string? Formulation,
    string? Company,
    int Quantity,
    string? Location,
    decimal? Price
);

public enum CatalogRowType
{
    Product,
    Package,
}
