using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.Models;

public class Product
{
    [Key]
    public Guid ProductId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Generic { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RetailPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesalePrice { get; set; }

    public int Stock { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public int LowStockThreshold { get; set; } = 10;

    public bool IsConsumable { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public bool HasExpiry { get; set; } = false;

    public Guid? BrandId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? FormulationId { get; set; }
    public Guid? CompanyId { get; set; }

    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public Formulation? Formulation { get; set; }
    public Company? Company { get; set; }
    public ConsumableExtension? Consumable { get; set; }
    public ICollection<InventoryBatch> Batches { get; set; } = [];
    public ICollection<BundleItem> BundleItems { get; set; } = [];

    [NotMapped]
    public bool IsLowStock => Stock <= LowStockThreshold;

    [NotMapped]
    public bool IsExpired => Batches.Count != 0 && Batches.Min(b => b.ExpiryDate) < DateTime.Now;
}
