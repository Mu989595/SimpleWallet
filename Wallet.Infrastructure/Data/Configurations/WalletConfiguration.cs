using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;
using WalletEntity = Wallet.Domain.Entities.Wallet; // Explicit alias to avoid ambiguity

namespace Wallet.Infrastructure.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.OwnsOne(w => w.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BalanceAmount")
                .HasColumnType("decimal(18,2)");
            
            money.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .HasMaxLength(3);
        });

        // Optimistic Concurrency
        builder.Property(w => w.RowVersion)
            .IsRowVersion();

        // One-to-Many relationship with Transactions
        // Since Transaction has source and destination wallet IDs, it's a bit complex.
        // For simplicity in this demo, we might just map the collection based on SourceWalletId? 
        // Or we treat Transaction as an Aggregate Root itself (which it is in our design).
        
        // Actually, Wallet owns the collection _transactions in the Domain, but in EF Core,
        // if Transaction is an Entity, it has its own table.
        // Let's configure the relationship:
        builder.HasMany(w => w.Transactions)
            .WithOne()
            .HasForeignKey("WalletId") // Shadow FK if we don't have it in Transaction
            .OnDelete(DeleteBehavior.Cascade);
            
        // Wait, Transaction entity has SourceWalletId and DestinationWalletId.
        // A transaction belongs to a history of a wallet.
        // Should we map based on SourceWalletId?
        // Let's leave navigation property configuration simple or omit if we query Transactions via TransactionRepository.
        // Domain model says: public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
        // This implies Wallet holds the transactions.
        // Let's map it firmly.
        
        builder.Metadata.FindNavigation(nameof(WalletEntity.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
