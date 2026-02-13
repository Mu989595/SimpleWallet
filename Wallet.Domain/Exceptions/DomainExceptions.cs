namespace Wallet.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(decimal currentBalance, decimal attemptedAmount)
        : base($"Insufficient funds. Balance: {currentBalance}, Attempted: {attemptedAmount}") { }
}

public class NegativeAmountException : DomainException
{
    public NegativeAmountException() : base("Amount must be greater than zero.") { }
}
