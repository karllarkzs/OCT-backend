namespace PharmaBack.WebApi.DTO.Restock;

public sealed record CreateRestockItemDto(
    Guid ProductId,
    int Quantity,
    decimal PurchasePrice,
    decimal RetailPrice
);
