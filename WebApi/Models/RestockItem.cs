using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.WebApi.Models;

public class ProductRestockItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductRestockId { get; set; }
    public ProductRestock ProductRestock { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RetailPrice { get; set; }
}
