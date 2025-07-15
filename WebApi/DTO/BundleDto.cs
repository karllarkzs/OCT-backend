namespace PharmaBack.WebApi.DTO;

public record PackageDto(
    Guid Id,
    string Barcode,
    string Name,
    decimal Price,
    string? Location, // Now nullable string
    List<PackageItemDto>? Items
);

public record PackageItemDto(Guid ProductId, int? Quantity, CatalogRowDto? ProductDetails);

public record AddPackageItemsRequest(Guid PackageId, List<PackageItemDto> Items);
