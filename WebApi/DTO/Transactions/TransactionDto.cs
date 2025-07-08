namespace PharmaBack.DTO.Transactions;

public sealed record TransactionSummaryDto(
    Guid Id,
    DateTime CreatedAt,
    decimal Total,
    int ItemCount
);

public sealed record TransactionDetailDto(
    Guid Id,
    DateTime CreatedAt,
    decimal Subtotal,
    decimal Vat,
    decimal? SpecialDiscount,
    decimal Total,
    List<TransactionItemDto> Items,
    bool IsVoided,
    string? VoidedBy,
    DateTime? VoidedAt,
    string? VoidReason
);

public sealed record TransactionItemDto(
    CatalogRowType ItemType,
    Guid CatalogId,
    string CatalogName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public sealed record TransactionFilterDto(
    Guid? Id = null,
    DateTime? From = null,
    DateTime? To = null,
    bool TodayOnly = false
);

public sealed record TransactionCreateDto(
    List<TransactionItemCreateDto> Items,
    decimal? CashInHand,
    decimal? Change,
    string ModeOfPayment,
    string? ReferenceNumber,
    decimal Subtotal,
    decimal Vat,
    decimal? SpecialDiscount,
    decimal Total,
    Guid? DiscountId
);

public sealed record TransactionItemCreateDto(
    Guid CatalogId,
    int Quantity,
    CatalogRowType? ItemType = null
);
