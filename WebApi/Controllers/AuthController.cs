using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaBack.DTO;
using PharmaBack.WebApi.Models;
using PharmaBack.WebApi.Services.Auth;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await authService.RegisterUserAsync(dto);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.Username);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await signInManager.PasswordSignInAsync(
            user,
            dto.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        var roles = await userManager.GetRolesAsync(user);

        return Ok(
            new
            {
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Email,
                Roles = roles,
            }
        );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        return Ok(
            new
            {
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Email,
                Roles = await userManager.GetRolesAsync(user),
            }
        );
    }
}
