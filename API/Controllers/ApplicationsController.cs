using System.Security.Claims;
using Application.Features.Applications.Commands.CancelApplication;
using Application.Features.Applications.Commands.CreateApplication;
using Application.Features.Applications.Commands.RespondApplication;
using Application.Features.Applications.Queries.GetApplications;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationsController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Входящие заявки (на мои предложения/запросы, ожидают ответа)</summary>
    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncoming([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(
            new GetApplicationsQuery(CurrentAccountId, ApplicationQueryType.Incoming, null, page, pageSize));
        return Ok(result);
    }

    /// <summary>Мои исходящие отклики</summary>
    [HttpGet("outgoing")]
    public async Task<IActionResult> GetOutgoing([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(
            new GetApplicationsQuery(CurrentAccountId, ApplicationQueryType.Outgoing, null, page, pageSize));
        return Ok(result);
    }

    /// <summary>Обработанные заявки (принятые / отклонённые) с фильтром</summary>
    [HttpGet("processed")]
    public async Task<IActionResult> GetProcessed(
        [FromQuery] ApplicationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(
            new GetApplicationsQuery(CurrentAccountId, ApplicationQueryType.Processed, status, page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Откликнуться на предложение (offerID) или на запрос (skillRequestID).
    /// Укажите ровно одно из двух полей.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] ApplyRequest request)
    {
        var id = await Mediator.Send(new CreateApplicationCommand(
            CurrentAccountId, request.OfferID, request.SkillRequestID, request.Message));
        return Created($"/api/applications/outgoing", new { id });
    }

    /// <summary>Отозвать свой отклик (только пока Pending)</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await Mediator.Send(new CancelApplicationCommand(id, CurrentAccountId));
        return NoContent();
    }

    /// <summary>Принять или отклонить заявку (только владелец предложения/запроса)</summary>
    [HttpPut("{id:guid}/respond")]
    public async Task<IActionResult> Respond(Guid id, [FromBody] RespondBody body)
    {
        await Mediator.Send(new RespondApplicationCommand(id, CurrentAccountId, body.Status));
        return NoContent();
    }
}

public record ApplyRequest(Guid? OfferID, Guid? SkillRequestID, string? Message);
public record RespondBody(ApplicationStatus Status);
