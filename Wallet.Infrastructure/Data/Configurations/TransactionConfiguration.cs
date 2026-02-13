using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.OwnsOne(t => t.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)");
            
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.Property(t => t.Type)
            .HasConversion<string>();

        builder.Property(t => t.SourceWalletId).IsRequired();
        builder.Property(t => t.DestinationWalletId);
    }
}
