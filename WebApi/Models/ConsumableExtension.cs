using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.WebApi.Models;

public class ConsumableExtension
{
    [Key, ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    public int UsesMax { get; set; }
    public int UsesLeft { get; set; }

    public Product Product { get; set; } = null!;
}
