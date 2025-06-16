using Microsoft.AspNetCore.Identity;
using PharmaBack.DTO;
using PharmaBack.Models;

namespace PharmaBack.Services.Auth;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterUserDto dto);
}

public class AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    : IAuthService
{
    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Username,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Photo = dto.Photo,
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return result;

        if (!await roleManager.RoleExistsAsync(dto.Role))
            await roleManager.CreateAsync(new IdentityRole(dto.Role));

        await userManager.AddToRoleAsync(user, dto.Role);

        return IdentityResult.Success;
    }
}
