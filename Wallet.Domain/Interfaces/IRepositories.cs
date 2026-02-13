using Wallet.Domain.Entities;
using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Domain.Interfaces;

public interface IWalletRepository
{
    Task<WalletEntity?> GetByIdAsync(Guid id);
    Task<WalletEntity?> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(WalletEntity wallet);
    Task AddAsync(WalletEntity wallet);
}

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetHistoryAsync(Guid walletId);
}
