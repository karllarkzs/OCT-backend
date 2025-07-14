using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmaBack.WebApi.DTO;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Services.Auth;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterUserDto dto);
    Task<AuthTokenDto> GenerateTokenAsync(AppUser user);
}

public sealed class AuthService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration
) : IAuthService
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

        if (result.Succeeded && !string.IsNullOrEmpty(dto.Role))
        {
            var roleExists = await roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
                await roleManager.CreateAsync(new IdentityRole(dto.Role));

            await userManager.AddToRoleAsync(user, dto.Role);
        }

        return result;
    }

    public async Task<AuthTokenDto> GenerateTokenAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = configuration["Jwt:Secret"];
        var issuer = configuration["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException("JWT configuration is invalid.");

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            expires: DateTime.UtcNow.AddHours(12),
            claims: claims,
            signingCredentials: new SigningCredentials(
                authSigningKey,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new AuthTokenDto(
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt: token.ValidTo,
            Username: user.UserName!,
            Roles: [.. roles]
        );
    }
}

public sealed record AuthTokenDto(
    string Token,
    DateTime ExpiresAt,
    string Username,
    string[] Roles
);
