using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO.Transactions;
using PharmaBack.WebApi.Services.Transactions;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] TransactionCreateDto dto,
        CancellationToken ct
    )
    {
        if (dto.Items == null || dto.Items.Count == 0)
            return BadRequest("Transaction must contain at least one item.");

        try
        {
            var transactionId = await transactionService.ProcessAsync(dto, ct);
            return CreatedAtAction(nameof(Query), new { id = transactionId }, transactionId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query(
        [FromBody] TransactionFilterDto filter,
        CancellationToken ct
    )
    {
        if (filter.Id is not null)
        {
            var detail = await transactionService.GetByIdAsync(filter.Id.Value, ct);
            return detail is null ? NotFound() : Ok(detail);
        }

        var results = await transactionService.FilterAsync(filter, ct);
        return Ok(results);
    }
}
