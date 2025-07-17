namespace PharmaBack.WebApi.DTO.Restock;

public sealed record CreateRestockDto(
    DateOnly ReceivedDate,
    string ReceivedBy,
    string? ReferenceNumber,
    string? SupplierName,
    List<CreateRestockItemDto> Items
);
