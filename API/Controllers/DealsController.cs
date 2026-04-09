using System.Security.Claims;
using Application.Features.Deals.Commands.CancelDeal;
using Application.Features.Deals.Commands.CompleteDeal;
using Application.Features.Deals.Queries.GetDealById;
using Application.Features.Deals.Queries.GetDeals;
using Application.Features.Deals.Queries.GetPublicDeals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/deals")]
public class DealsController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Мои сделки (история)</summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDeals([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetDealsQuery(CurrentAccountId, page, pageSize));
        return Ok(result);
    }

    /// <summary>Публичная история завершённых/отменённых сделок пользователя</summary>
    [HttpGet("user/{accountId:guid}")]
    public async Task<IActionResult> GetUserDeals(
        Guid accountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPublicDealsQuery(accountId, page, pageSize));
        return Ok(result);
    }

    /// <summary>Детали сделки (только участники)</summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetDealByIdQuery(id, CurrentAccountId));
        return Ok(result);
    }

    /// <summary>Отметить сделку выполненной со своей стороны</summary>
    [HttpPut("{id:guid}/complete")]
    [Authorize]
    public async Task<IActionResult> Complete(Guid id)
    {
        await Mediator.Send(new CompleteDealCommand(id, CurrentAccountId));
        return NoContent();
    }

    /// <summary>Отменить сделку</summary>
    [HttpPut("{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await Mediator.Send(new CancelDealCommand(id, CurrentAccountId));
        return NoContent();
    }
}
