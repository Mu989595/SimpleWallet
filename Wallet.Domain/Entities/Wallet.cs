using Wallet.Domain.Exceptions;
using Wallet.Domain.ValueObjects;
using Wallet.Domain.Events;

namespace Wallet.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Money Balance { get; private set; } = null!;
    public byte[] RowVersion { get; private set; } = null!; // Optimistic Concurrency

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    // Constructor for EF Core
    private Wallet() { }

    public Wallet(Guid userId, string currency)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Balance = Money.Zero(currency);
    }

    public void Deposit(Money amount)
    {
        if (amount.Amount <= 0)
            throw new NegativeAmountException();

        Balance += amount;
        
        _transactions.Add(new Transaction(amount, TransactionType.Deposit, Id));
    }

    public void Withdraw(Money amount)
    {
        if (amount.Amount <= 0)
            throw new NegativeAmountException();

        if (Balance.Amount < amount.Amount)
            throw new InsufficientFundsException(Balance.Amount, amount.Amount);

        Balance -= amount;
        
        _transactions.Add(new Transaction(amount, TransactionType.Withdraw, Id));
    }

    public void Transfer(Wallet destination, Money amount)
    {
        if (amount.Amount <= 0)
            throw new NegativeAmountException();

        if (Balance.Amount < amount.Amount)
            throw new InsufficientFundsException(Balance.Amount, amount.Amount);

        Balance -= amount;
        
        // Record the outgoing transfer
        _transactions.Add(new Transaction(amount, TransactionType.Transfer, Id, destination.Id));
        
        // Execute the deposit on the destination
        destination.ReceiveTransfer(amount, Id);
    }
    
    public void ReceiveTransfer(Money amount, Guid sourceWalletId)
    {
        if (amount.Amount <= 0)
             throw new NegativeAmountException();

        Balance += amount;
        
        // Record the incoming transfer
        _transactions.Add(new Transaction(amount, TransactionType.Transfer, sourceWalletId, Id));
    }
}
