namespace PharmaBack.DTO;

public record BundleDto(
    Guid BundleId,
    string Code,
    string Name,
    decimal Price,
    List<BundleItemDto> Items
);

public record BundleItemDto(Guid ProductId, int Quantity);
