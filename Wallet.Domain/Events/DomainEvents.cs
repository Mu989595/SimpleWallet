using Wallet.Domain.Entities;

namespace Wallet.Domain.Events;

// Base class for all domain events
public abstract record DomainEvent(Guid EventId, DateTime OccurredOn);

public record TransactionCreatedEvent(Transaction Transaction) 
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
