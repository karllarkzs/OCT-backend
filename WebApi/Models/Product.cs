using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.WebApi.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Generic { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RetailPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesalePrice { get; set; }

    public int Stock { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    public bool IsConsumable { get; set; }
    public bool HasExpiry { get; set; }
    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Formulation { get; set; }

    [StringLength(100)]
    public string? Company { get; set; }

    public ConsumableExtension? Consumable { get; set; }
    public ICollection<InventoryBatch> Batches { get; set; } = [];
    public ICollection<BundleItem> BundleItems { get; set; } = [];

    [NotMapped]
    public bool IsLowStock => Stock <= LowStockThreshold;

    [NotMapped]
    public bool IsExpired =>
        Batches.Any(b => b.ExpiryDate.HasValue)
        && Batches.Where(b => b.ExpiryDate.HasValue).Min(b => b.ExpiryDate!.Value)
            < DateOnly.FromDateTime(DateTime.Today);
}
