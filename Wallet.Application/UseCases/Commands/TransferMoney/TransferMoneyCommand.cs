using FluentValidation;
using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Interfaces;
using Wallet.Domain.ValueObjects;

namespace Wallet.Application.UseCases.Commands.TransferMoney;

// Command
public record TransferMoneyCommand(Guid SourceUserId, Guid DestinationUserId, decimal Amount, string Currency) : ICommand;

// Validator
public class TransferMoneyCommandValidator : AbstractValidator<TransferMoneyCommand>
{
    public TransferMoneyCommandValidator()
    {
        RuleFor(x => x.SourceUserId).NotEmpty();
        RuleFor(x => x.DestinationUserId).NotEmpty().NotEqual(x => x.SourceUserId);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

// Handler
public class TransferMoneyCommandHandler : IRequestHandler<TransferMoneyCommand, Result>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferMoneyCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(TransferMoneyCommand request, CancellationToken cancellationToken)
    {
        var sourceWallet = await _walletRepository.GetByUserIdAsync(request.SourceUserId);
        if (sourceWallet is null)
        {
            return Result.Failure("Source wallet not found.");
        }

        var destinationWallet = await _walletRepository.GetByUserIdAsync(request.DestinationUserId);
        if (destinationWallet is null)
        {
            return Result.Failure("Destination wallet not found.");
        }

        var amount = new Money(request.Amount, request.Currency);

        try
        {
            // Domain Logic
            sourceWallet.Transfer(destinationWallet, amount);

            // Persistence
            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(destinationWallet);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DomainException ex)
        {
             return Result.Failure(ex.Message);
        }

        return Result.Success();
    }
}
