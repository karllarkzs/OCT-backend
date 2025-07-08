using System.ComponentModel.DataAnnotations;
using PharmaBack.DTO;

namespace PharmaBack.WebApi.Models;

public class TransactionItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public Guid CatalogId { get; set; }
    public CatalogRowType ItemType { get; set; }

    public string CatalogName { get; set; } = null!;
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
