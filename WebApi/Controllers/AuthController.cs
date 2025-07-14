using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.Models;
using PharmaBack.WebApi.Services.Auth;

namespace PharmaBack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<AppUser> userManager, IAuthService authService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var existingUser = await userManager.FindByNameAsync(dto.Username);
        if (existingUser is not null)
            return BadRequest("Username already exists.");

        var result = await authService.RegisterUserAsync(dto);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] SignInDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.Username);
        if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid credentials");

        var tokenDto = await authService.GenerateTokenAsync(user);
        return Ok(tokenDto);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { Message = "Logged out (client should discard the token)." });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized();

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
            return NotFound();

        var roles = await userManager.GetRolesAsync(user);

        return Ok(
            new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Photo,
                Roles = roles,
            }
        );
    }
}
