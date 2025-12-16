using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserProfileRepository _users;
    private readonly IPasswordHasher<UserProfile> _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthController(
        IUserProfileRepository users,
        IPasswordHasher<UserProfile> hasher,
        IJwtTokenService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing != null) return Conflict("Email already exists");

        var user = new UserProfile
        {
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            NeighborhoodId = dto.NeighborhoodId,
            Role = Role.User
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        var saved = await _users.AddAsync(user);
        if (!saved) return StatusCode(500, "Could not create user");

        var token = _jwt.CreateToken(user.Id, user.Email, user.Role.ToString());

        return Ok(new AuthResponseDto
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _users.GetByEmailAsync(dto.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized("Invalid credentials");

        var token = _jwt.CreateToken(user.Id, user.Email, user.Role.ToString());

        return Ok(new AuthResponseDto
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        });
    }
}
