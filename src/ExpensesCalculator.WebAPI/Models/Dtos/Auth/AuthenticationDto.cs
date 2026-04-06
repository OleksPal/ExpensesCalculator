using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.WebAPI.Models.Dtos.Auth;

public record RegisterRequest(
    [Required(ErrorMessage = "Username is required.")] string UserName,
    [Required(ErrorMessage = "Password is required.")] string Password
);

public record LoginRequest(
    [Required(ErrorMessage = "Username is required.")] string UserName,
    [Required(ErrorMessage = "Password is required.")] string Password
);

public record AuthResponse(string AccessToken, string RefreshToken);
public record RefreshRequest(string RefreshToken);
