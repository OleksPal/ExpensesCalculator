namespace ExpensesCalculator.WebAPI.Models.Dtos;

public record RegisterRequest(string UserName, string Password);
public record LoginRequest(string UserName, string Password);
public record AuthResponse(string AccessToken, string RefreshToken);
public record RefreshRequest(string RefreshToken);