using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.Models;

public class InventoryBatch
{
    [Key]
    public Guid BatchId { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    [StringLength(100)]
    public string? LotNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }
    public int QuantityOnHand { get; set; }

    [StringLength(100)]
    public string? StorageLocation { get; set; }

    public Product Product { get; set; } = null!;
}
