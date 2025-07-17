using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.WebApi.Models;

public class ProductRestock
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateOnly ReceivedDate { get; set; }

    public string? ReferenceNumber { get; set; }
    public string? SupplierName { get; set; }

    public string ReceivedBy { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; } // Sum of all item (PurchasePrice * Quantity)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductRestockItem> Items { get; set; } = new List<ProductRestockItem>();
}
