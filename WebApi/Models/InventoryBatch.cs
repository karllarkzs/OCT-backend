using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaBack.WebApi.Models;

public class InventoryBatch
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public DateOnly? ExpiryDate { get; set; }
    public int QuantityOnHand { get; set; }
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    public Product Product { get; set; } = null!;
}
