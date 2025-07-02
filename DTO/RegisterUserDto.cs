namespace PharmaBack.DTO;

public class RegisterUserDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public byte[]? Photo { get; set; }
    public string Role { get; set; } = "cashier";
}
