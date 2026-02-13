using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Interfaces;

namespace Wallet.Application.UseCases.Queries.GetWallet;

// DTO
public record WalletDto(Guid Id, Guid UserId, decimal Balance, string Currency, List<TransactionDto> Transactions);

public record TransactionDto(decimal Amount, string Type, DateTime Date);

// Query
public record GetWalletQuery(Guid UserId) : IQuery<WalletDto>;

// Handler
public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, Result<WalletDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);

        if (wallet is null)
        {
            return Result<WalletDto>.Failure("Wallet not found.");
        }

        // Mapping (Manual for now, can use AutoMapper later)
        var transactions = wallet.Transactions
            .Select(t => new TransactionDto(t.Amount.Amount, t.Type.ToString(), t.Timestamp))
            .ToList();

        var dto = new WalletDto(
            wallet.Id,
            wallet.UserId,
            wallet.Balance.Amount,
            wallet.Balance.Currency,
            transactions);

        return Result<WalletDto>.Success(dto);
    }
}
