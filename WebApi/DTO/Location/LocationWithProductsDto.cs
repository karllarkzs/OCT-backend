using PharmaBack.DTO.Product;

namespace PharmaBack.DTO.Location;

public sealed record LocationWithProducsDto(
    Guid Id,
    string Name,
    IReadOnlyList<GetProductDto> Products
);
