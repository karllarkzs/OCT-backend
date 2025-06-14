using System.ComponentModel.DataAnnotations;

namespace PharmaBack.Models;

public class Company
{
    [Key]
    public Guid CompanyId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
