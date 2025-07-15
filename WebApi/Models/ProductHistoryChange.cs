using System.ComponentModel.DataAnnotations;

namespace PharmaBack.WebApi.Models;

public class ProductHistoryChange
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductHistoryId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public ProductHistory ProductHistory { get; set; } = null!;
}
