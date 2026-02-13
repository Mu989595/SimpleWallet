using Wallet.Domain.Interfaces;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WalletDbContext _dbContext;

    public UnitOfWork(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
