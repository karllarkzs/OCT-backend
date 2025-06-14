namespace PharmaBack.DTO;

public sealed record CatalogRowDto(
    Guid Id,
    string RowType,
    string Code,
    string Name,
    DateTime? Expiry,
    int Quantity,
    string? Location,
    decimal? Price
);
