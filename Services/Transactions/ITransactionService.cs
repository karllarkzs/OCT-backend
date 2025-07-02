using PharmaBack.DTO.Transactions;

namespace PharmaBack.Services.Transactions;

public interface ITransactionService
{
    Task<Guid> ProcessAsync(TransactionCreateDto dto, CancellationToken ct);
    Task<TransactionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<TransactionSummaryDto>> FilterAsync(
        TransactionFilterDto filter,
        CancellationToken ct
    );
}
