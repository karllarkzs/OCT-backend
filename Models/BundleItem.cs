using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmaBack.Models;

[Index(nameof(BundleId), nameof(ProductId), nameof(InventoryBatchId), IsUnique = true)]
public class BundleItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BundleId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public Guid InventoryBatchId { get; set; } // 👈 NEW FIELD

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int Uses { get; set; } = 0;

    public Bundle Bundle { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public InventoryBatch InventoryBatch { get; set; } = null!; // 👈 NEW NAV PROP
}
