using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Products;

public interface IProductAuditService
{
    Task LogChangeAsync(
        Product product,
        ProductActionType actionType,
        IEnumerable<string> changedFields,
        string userId,
        string userName,
        CancellationToken ct = default
    );
}
