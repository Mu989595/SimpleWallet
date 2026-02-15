namespace Wallet.Application.DTOs;

public record RegisterDto(string Email, string Password, string FullName);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, DateTime Expiration, Guid UserId, string Email, string FullName);
