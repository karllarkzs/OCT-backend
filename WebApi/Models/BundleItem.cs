using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmaBack.WebApi.Models;

[Index(nameof(BundleId), nameof(ProductId), IsUnique = true)]
public class BundleItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BundleId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 0;

    public Bundle Bundle { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
