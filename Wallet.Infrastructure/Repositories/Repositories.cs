using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;
using Wallet.Domain.Interfaces;
using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly Data.WalletDbContext _context;

    public WalletRepository(Data.WalletDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WalletEntity wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }

    public async Task<WalletEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WalletEntity?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public Task UpdateAsync(WalletEntity wallet)
    {
        _context.Wallets.Update(wallet);
        return Task.CompletedTask;
    }
}

public class TransactionRepository : ITransactionRepository
{
    private readonly Data.WalletDbContext _context;

    public TransactionRepository(Data.WalletDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task<IEnumerable<Transaction>> GetHistoryAsync(Guid walletId)
    {
        return await _context.Transactions
            .Where(t => t.SourceWalletId == walletId || t.DestinationWalletId == walletId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }
}
