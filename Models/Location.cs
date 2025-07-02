using System.ComponentModel.DataAnnotations;

namespace PharmaBack.Models;

public class Location : ISoftDelete
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public ICollection<InventoryBatch> InventoryBatches { get; set; } = [];
}
