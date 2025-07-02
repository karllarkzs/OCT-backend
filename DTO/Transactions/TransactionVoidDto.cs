namespace PharmaBack.DTO.Transactions;

public sealed record TransactionVoidDto(Guid TransactionId, string? Reason = null);
