using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO.Product;
using PharmaBack.WebApi.Services.Products;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<GetProductDto>> GetAll(CancellationToken ct) =>
        await svc.GetAllAsync(ct);

    [HttpPost]
    public async Task<IReadOnlyList<Guid>> Add(
        IEnumerable<CreateProductDto> dtos,
        CancellationToken ct
    ) => await svc.BatchCreateOrUpdateAsync(dtos, ct);
}
