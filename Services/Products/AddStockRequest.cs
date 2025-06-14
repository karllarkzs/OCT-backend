namespace PharmaBack.Services.Products;

public sealed record AddStockRequest(
    string Barcode,
    string Name,
    decimal RetailPrice,
    decimal WholesalePrice,
    int Quantity,
    bool HasExpiry,
    DateTime? ExpiryDate,
    bool IsConsumable
);
