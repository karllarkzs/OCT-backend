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

    public int Quantity { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? ReceivedDate { get; set; }
    public string? Location { get; set; }
    public int MinStock { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsDiscountable { get; set; } = true;
    public bool IsReagent { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Formulation { get; set; }

    [StringLength(100)]
    public string? Company { get; set; }

    [StringLength(100)]
    public string? Type { get; set; }

    public ICollection<PackageItem> PackageItems { get; set; } = [];
    public ICollection<ProductHistory> History { get; set; } = [];

    [NotMapped]
    public bool IsLowStock => Quantity <= MinStock;

    [NotMapped]
    public bool IsExpired =>
        ExpiryDate.HasValue && ExpiryDate.Value < DateOnly.FromDateTime(DateTime.Today);
}
