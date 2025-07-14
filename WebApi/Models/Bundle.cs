namespace PharmaBack.WebApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Bundle
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsDeleted { get; set; } = false;
    public string? Location { get; set; }

    public ICollection<BundleItem> BundleItems { get; set; } = [];
}
