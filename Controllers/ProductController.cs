using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO;
using PharmaBack.DTO.Product;
using PharmaBack.Services.Products;

namespace PharmaBack.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<GetProductDto>> Get(string? search, CancellationToken ct) =>
        await svc.GetAsync(search, ct);

    [HttpGet("all")]
    public async Task<IReadOnlyList<GetProductDto>> GetAll(CancellationToken ct) =>
        await svc.GetAllAsync(ct);

    [HttpPost]
    public async Task<Guid> Add(AddStockRequestDto dto, CancellationToken ct) =>
        await svc.AddOrUpdateAsync(dto, ct);

    [HttpPost("batch")]
    public async Task<IReadOnlyList<Guid>> AddBatch(
        IEnumerable<AddStockRequestDto> dtos,
        CancellationToken ct
    ) => await svc.BatchAddOrUpdateAsync(dtos, ct);
}
