using System.ComponentModel.DataAnnotations;

namespace PharmaBack.WebApi.Models;

public enum ProductActionType
{
    Added,
    Updated,
    Deleted,
    Sold,
    Restocked,
    Voided,
}

public class ProductHistory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public string Barcode { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? Generic { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public int Quantity { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? ReceivedDate { get; set; }
    public string? Location { get; set; }
    public int MinStock { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsDiscountable { get; set; }
    public string? Category { get; set; }
    public string? Formulation { get; set; }
    public string? Company { get; set; }
    public string? Type { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public ProductActionType ActionType { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public string ChangedByUserName { get; set; } = string.Empty;

    public ICollection<ProductHistoryChange> Changes { get; set; } = [];
}
