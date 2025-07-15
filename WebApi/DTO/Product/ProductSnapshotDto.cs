using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.DTO.Product;

public sealed record ProductSnapshotDto(
    Guid Id,
    Guid ProductId,
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
    bool IsDeleted,
    bool IsDiscountable,
    string? Category,
    string? Formulation,
    string? Company,
    string? Type,
    DateTime ChangedAt,
    ProductActionType ActionType,
    string ChangedByUserId,
    string ChangedByUserName,
    IReadOnlyList<string> Changes
);
