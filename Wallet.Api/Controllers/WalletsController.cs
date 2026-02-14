using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.UseCases.Commands.CreateWallet;
using Wallet.Application.UseCases.Commands.DepositMoney;
using Wallet.Application.UseCases.Commands.TransferMoney;
using Wallet.Application.UseCases.Commands.WithdrawMoney;
using Wallet.Application.UseCases.Queries.GetWallet;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly ISender _sender;

    public WalletsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetWallet), new { userId = command.UserId }, result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWallet(Guid userId)
    {
        var query = new GetWalletQuery(userId);
        var result = await _sender.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositMoneyCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsSuccess)
        {
            return Ok("Deposit successful.");
        }

        return BadRequest(result.Error);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawMoneyCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsSuccess)
        {
            return Ok("Withdrawal successful.");
        }

        return BadRequest(result.Error);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferMoneyCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsSuccess)
        {
            return Ok("Transfer successful.");
        }

        return BadRequest(result.Error);
    }
}

