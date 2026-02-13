using FluentValidation;
using MediatR;
using Wallet.Application.Abstractions.Messaging;
using Wallet.Domain.Entities;
using Wallet.Domain.Interfaces;
using Wallet.Domain.ValueObjects;

namespace Wallet.Application.UseCases.Commands.CreateWallet;

// Command
public record CreateWalletCommand(Guid UserId, string Currency) : ICommand<Guid>;

// Validator
public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

// Handler
public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<Guid>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        // Check if wallet already exists for user? (Optional business rule)
        var existingWallet = await _walletRepository.GetByUserIdAsync(request.UserId);
        if (existingWallet is not null)
        {
            return Result<Guid>.Failure("User already has a wallet.");
        }

        var wallet = new Wallet.Domain.Entities.Wallet(request.UserId, request.Currency);

        await _walletRepository.AddAsync(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(wallet.Id);
    }
}
