namespace Wallet.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string Role { get; private set; } = "User";
    public DateTime CreatedAt { get; private set; }

    private User() { } // EF Core

    public User(Guid id, string email, string passwordHash, string fullName)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string email, string passwordHash, string fullName)
    {
        return new User(Guid.NewGuid(), email, passwordHash, fullName);
    }
}
