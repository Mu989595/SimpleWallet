using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Interfaces;

namespace Wallet.Application.UseCases.Queries.GetWallet;

// DTOs
public record MoneyDto(decimal Amount, string Currency);

public record WalletDto(Guid Id, Guid UserId, MoneyDto Balance, List<TransactionDto> Transactions);

public record TransactionDto(Guid Id, MoneyDto Amount, string Type, DateTime Timestamp);

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

        var transactions = wallet.Transactions
            .Select(t => new TransactionDto(
                t.Id,
                new MoneyDto(t.Amount.Amount, t.Amount.Currency),
                t.Type.ToString(),
                t.Timestamp))
            .ToList();

        var dto = new WalletDto(
            wallet.Id,
            wallet.UserId,
            new MoneyDto(wallet.Balance.Amount, wallet.Balance.Currency),
            transactions);

        return Result<WalletDto>.Success(dto);
    }
}

