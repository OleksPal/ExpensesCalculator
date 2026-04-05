using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos.Auth;
using ExpensesCalculator.WebAPI.Services.Auth;
using ExpensesCalculator.WebAPI.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwt;
    private readonly ExpensesContext _context;
    private readonly PasswordHasher<User> _hasher;

    public AuthController(JwtTokenService jwt, ExpensesContext context)
    {
        _jwt = jwt;
        _context = context;
        _hasher = new PasswordHasher<User>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (_context.Users.Any(u => u.UserName == request.UserName))
            return BadRequest(new IdentityError[]
            {
                new IdentityError
                {
                    Code = "UserExists",
                    Description = "User already exists."
                }
            });

        var validationErrors = PasswordValidator.Validate(request.Password);
        if (validationErrors.Length > 0)
            return BadRequest(validationErrors);

        var user = new User
        {
            UserName = request.UserName,
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if (user == null)
            return Unauthorized(new IdentityError[]
            {
                new IdentityError
                {
                    Code = "InvalidCredentials",
                    Description = "Invalid email or password."
                }
            });

        var passwordCheck = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (!passwordCheck.HasFlag(PasswordVerificationResult.Success))
            return Unauthorized(new IdentityError[]
            {
                new IdentityError
                {
                    Code = "InvalidCredentials",
                    Description = "Invalid email or password."
                }
            });

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(accessToken, refreshToken.Token));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var token = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.IsRevoked && t.Expires > DateTime.UtcNow);

        if (token == null)
            return Unauthorized();

        token.IsRevoked = true;

        var newAccessToken = _jwt.GenerateAccessToken(token.User);
        var newRefreshToken = _jwt.GenerateRefreshToken(token.UserId);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(newAccessToken, newRefreshToken.Token));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId.ToString() == userId);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        return Ok();
    }
}

