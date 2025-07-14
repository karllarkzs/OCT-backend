using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO.Transactions;

namespace PharmaBack.WebApi.Services.Transactions;

public interface ITransactionService
{
    Task<Guid> ProcessAsync(TransactionCreateDto dto, CancellationToken ct);
    Task<TransactionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<TransactionSummaryDto>> FilterAsync(
        TransactionFilterDto filter,
        CancellationToken ct
    );
    Task VoidTransactionAsync(
        TransactionVoidDto dto,
        string? username,
        CancellationToken ct = default
    );
}
