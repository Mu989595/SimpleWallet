using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;
using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Infrastructure.Data;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
