namespace PharmaBack.WebApi.DTO;

public sealed record BrandDto(Guid Id, string Name);

public sealed record UpsertBrandDto(string Name);
