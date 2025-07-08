using System.ComponentModel.DataAnnotations;

namespace PharmaBack.WebApi.Models;

public class Discount
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    public decimal? Percentage { get; set; }
    // public List<DiscountExclusion> ExcludedItems { get; set; } = [];
}
