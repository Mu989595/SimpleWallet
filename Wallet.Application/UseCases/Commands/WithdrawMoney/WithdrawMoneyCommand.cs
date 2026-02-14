using FluentValidation;
using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Interfaces;
using Wallet.Domain.ValueObjects;

namespace Wallet.Application.UseCases.Commands.WithdrawMoney;

// Command
public record WithdrawMoneyCommand(Guid UserId, decimal Amount, string Currency) : ICommand;

// Validator
public class WithdrawMoneyCommandValidator : AbstractValidator<WithdrawMoneyCommand>
{
    public WithdrawMoneyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

// Handler
public class WithdrawMoneyCommandHandler : IRequestHandler<WithdrawMoneyCommand, Result>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawMoneyCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
        if (wallet is null)
        {
            return Result.Failure("Wallet not found.");
        }

        var amount = new Money(request.Amount, request.Currency);

        try
        {
            wallet.Withdraw(amount);
            await _walletRepository.UpdateAsync(wallet);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }
}
