using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.DTO.Transactions;
using PharmaBack.WebApi.Services.Transactions;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/transactions")]
public sealed class VoidTransactionController(ITransactionService transactionService)
    : ControllerBase
{
    [HttpPost("void")]
    [Authorize]
    public async Task<IActionResult> VoidTransaction(
        [FromBody] TransactionVoidDto dto,
        CancellationToken ct
    )
    {
        var username = User?.Identity?.Name;

        try
        {
            await transactionService.VoidTransactionAsync(dto, username, ct);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
