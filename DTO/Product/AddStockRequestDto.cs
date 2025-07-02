namespace PharmaBack.DTO.Product;

public sealed record AddStockRequestDto(
    Guid? Id,
    string? Barcode,
    string? Generic,
    string? Brand,
    decimal RetailPrice,
    decimal WholesalePrice,
    int Quantity,
    bool HasExpiry,
    DateOnly? ExpiryDate,
    bool IsConsumable,
    int? UsesMax = null,
    int? UsesLeft = null,
    string? Category = null,
    string? Formulation = null,
    string? Company = null,
    Guid? LocationId = null
);
