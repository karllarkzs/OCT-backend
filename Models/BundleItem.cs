using System.ComponentModel.DataAnnotations;

namespace PharmaBack.Models;

public class BundleItem
{
    [Key]
    public Guid BundleItemId { get; set; } = Guid.NewGuid();

    public Guid BundleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public Bundle Bundle { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
