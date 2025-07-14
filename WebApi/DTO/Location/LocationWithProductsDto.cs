using PharmaBack.WebApi.DTO.Product;

namespace PharmaBack.WebApi.DTO.Location;

public sealed record LocationWithProducsDto(
    Guid Id,
    string Name,
    IReadOnlyList<GetProductDto> Products
);
