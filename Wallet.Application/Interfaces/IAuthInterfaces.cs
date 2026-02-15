using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTime Expiration) GenerateToken(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
