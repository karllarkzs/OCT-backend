using System.ComponentModel.DataAnnotations;

namespace PharmaBack.WebApi.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal? CashInHand { get; set; }
    public decimal? Change { get; set; }
    public string? ModeOfPayment { get; set; }
    public string? ReferenceNumber { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Vat { get; set; }
    public decimal? SpecialDiscount { get; set; }
    public decimal Total { get; set; }

    public Guid? DiscountId { get; set; }
    public Discount? Discount { get; set; }

    public bool IsVoided { get; set; }
    public string? VoidedBy { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string? VoidReason { get; set; }

    public List<TransactionItem> Items { get; set; } = new();
}
