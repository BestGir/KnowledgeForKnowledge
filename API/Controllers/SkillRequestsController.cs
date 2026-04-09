using System.Security.Claims;
using Application.Features.SkillRequests.Commands.CreateSkillRequest;
using Application.Features.SkillRequests.Commands.DeleteSkillRequest;
using Application.Features.SkillRequests.Commands.UpdateSkillRequestStatus;
using Application.Features.SkillRequests.Queries.GetSkillRequestById;
using Application.Features.SkillRequests.Queries.GetSkillRequests;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/skillrequests")]
public class SkillRequestsController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Список запросов с фильтрацией и пагинацией (публичный)</summary>
    [HttpGet]
    public async Task<IActionResult> GetRequests(
        [FromQuery] Guid? accountId,
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetSkillRequestsQuery(accountId, status, page, pageSize));
        return Ok(result);
    }

    /// <summary>Получить запрос по ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetSkillRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>Создать запрос на изучение навыка</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRequest([FromBody] CreateRequestBody request)
    {
        var id = await Mediator.Send(
            new CreateSkillRequestCommand(CurrentAccountId, request.SkillID, request.Title, request.Details));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Изменить статус запроса (Closed / OnHold / Open)</summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusBody body)
    {
        await Mediator.Send(new UpdateSkillRequestStatusCommand(id, CurrentAccountId, body.Status));
        return NoContent();
    }

    /// <summary>Удалить запрос (только автор)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteSkillRequestCommand(id, CurrentAccountId));
        return NoContent();
    }
}

public class CreateRequestBody
{
    public Guid SkillID { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Details { get; init; }
}

public record UpdateStatusBody(RequestStatus Status);
