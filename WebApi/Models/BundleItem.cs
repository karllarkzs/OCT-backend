using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmaBack.WebApi.Models;

[Index(nameof(PackageId), nameof(ProductId), IsUnique = true)]
public class PackageItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PackageId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 0;

    public Package Package { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
