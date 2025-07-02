using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO.Transactions;

namespace PharmaBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class VoidTransactionController(PharmaDbContext db) : ControllerBase
{
    [HttpPost("void")]
    public async Task<IActionResult> VoidTransaction(
        [FromBody] TransactionVoidDto dto,
        CancellationToken ct
    )
    {
        var transaction = await db.Transactions.FirstOrDefaultAsync(
            t => t.Id == dto.TransactionId,
            ct
        );
        if (transaction is null)
            return NotFound("Transaction not found.");

        if (transaction.IsVoided)
            return BadRequest("Transaction is already voided.");

        var username = User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized("Could not determine user identity.");

        transaction.IsVoided = true;
        transaction.VoidedBy = username;
        transaction.VoidedAt = DateTime.UtcNow;
        transaction.VoidReason = dto.Reason;

        await db.SaveChangesAsync(ct);

        return Ok();
    }
}
