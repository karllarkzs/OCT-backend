namespace PharmaBack.DTO;

public record BundleDto(
    Guid Id,
    string Barcode,
    string Name,
    decimal Price,
    List<BundleItemDto>? Items
);

public record BundleItemDto(
    Guid ProductId,
    Guid InventoryBatchId, // 👈 NEW
    int? Quantity,
    int? Uses
);

public record AddBundleItemsRequest(Guid BundleId, List<BundleItemDto> Items);
