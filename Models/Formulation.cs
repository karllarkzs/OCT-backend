using System.ComponentModel.DataAnnotations;

namespace PharmaBack.Models;

public class Formulation
{
    [Key]
    public Guid FormulationId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = [];
}
