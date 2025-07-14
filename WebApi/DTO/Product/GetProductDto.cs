namespace PharmaBack.WebApi.DTO.Product;

public sealed record GetProductDto(
    Guid Id,
    string? Barcode,
    string? Generic,
    string? Brand,
    string? Category,
    string? Formulation,
    string? Company,
    string? Type,
    decimal RetailPrice,
    decimal WholesalePrice,
    int Quantity,
    int MinStock,
    DateOnly? ExpiryDate,
    string? Location,
    DateOnly? ReceivedDate,
    bool IsExpired,
    bool IsLowStock
);
