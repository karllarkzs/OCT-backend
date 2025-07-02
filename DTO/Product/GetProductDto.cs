namespace PharmaBack.DTO.Product;

public sealed record GetProductDto(
    Guid Id,
    Guid BatchId,
    string Barcode,
    string? Generic,
    string? Brand,
    string? Category,
    string? Formulation,
    string? Company,
    decimal RetailPrice,
    decimal WholesalePrice,
    int QuantityOnHand,
    DateOnly? ExpiryDate,
    string? LocationName,
    Guid? LocationId,
    bool IsConsumable,
    int? UsesMax,
    int? UsesLeft
);
