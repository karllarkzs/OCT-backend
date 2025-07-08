using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PharmaBack.WebApi.Models;

public class AppUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = default!;

    public byte[]? Photo { get; set; }
}
