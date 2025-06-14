namespace PharmaBack.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Bundle
{
    [Key]
    public Guid BundleId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<BundleItem> BundleItems { get; set; } = new List<BundleItem>();
}
