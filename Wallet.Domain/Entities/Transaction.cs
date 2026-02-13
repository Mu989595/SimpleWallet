using Wallet.Domain.ValueObjects;

namespace Wallet.Domain.Entities;

public enum TransactionType
{
    Deposit,
    Withdraw,
    Transfer
}

public class Transaction
{
    public Guid Id { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Guid SourceWalletId { get; private set; }
    public Guid? DestinationWalletId { get; private set; }

    // Constructor for EF Core
    private Transaction() { }

    public Transaction(
        Money amount, 
        TransactionType type, 
        Guid sourceWalletId, 
        Guid? destinationWalletId = null)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        Timestamp = DateTime.UtcNow;
        SourceWalletId = sourceWalletId;
        DestinationWalletId = destinationWalletId;
    }
}
