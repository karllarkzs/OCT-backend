namespace PharmaBack.WebApi.DTO;

public record BundleDto(
    Guid Id,
    string Barcode,
    string Name,
    decimal Price,
    string? Location, // Now nullable string
    List<BundleItemDto>? Items
);

public record BundleItemDto(Guid ProductId, int? Quantity, CatalogRowDto? ProductDetails);

public record AddBundleItemsRequest(Guid BundleId, List<BundleItemDto> Items);
