namespace PharmaBack.WebApi.DTO.Product;

public sealed record RestockProductDto(
    Guid? Id,
    string Barcode,
    string Brand,
    string? Generic,
    decimal RetailPrice,
    decimal WholesalePrice,
    int Quantity,
    DateOnly? ExpiryDate,
    DateOnly? ReceivedDate,
    string? Location,
    int MinStock,
    string? Category,
    string? Formulation,
    string? Company,
    string? Type,
    bool IsDiscountable,
    bool IsReagent
);
