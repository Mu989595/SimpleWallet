using FluentValidation;
using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Interfaces;
using Wallet.Domain.ValueObjects;

namespace Wallet.Application.UseCases.Commands.DepositMoney;

// Command
public record DepositMoneyCommand(Guid UserId, decimal Amount, string Currency) : ICommand;

// Validator
public class DepositMoneyCommandValidator : AbstractValidator<DepositMoneyCommand>
{
    public DepositMoneyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

// Handler
public class DepositMoneyCommandHandler : IRequestHandler<DepositMoneyCommand, Result>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositMoneyCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
        if (wallet is null)
        {
            return Result.Failure("Wallet not found.");
        }

        var amount = new Money(request.Amount, request.Currency);

        try
        {
            wallet.Deposit(amount);
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
