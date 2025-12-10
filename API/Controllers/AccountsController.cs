using Application.Features.Accounts.Commands.CreateAccount;
using Application.Features.Accounts.Queries.GetAccount;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class AccountsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var accountId = await Mediator.Send(command);
        return Ok(accountId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
    {
        var query = new GetAccountQuery { AccountID = id };
        var account = await Mediator.Send(query);
        return Ok(account);
    }
}


