using System.Security.Claims;
using Application.Features.Notifications.Commands.MarkNotificationsRead;
using Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Мои уведомления</summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var result = await Mediator.Send(
            new GetNotificationsQuery(CurrentAccountId, unreadOnly, page, pageSize));
        return Ok(result);
    }

    /// <summary>Пометить одно уведомление прочитанным</summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await Mediator.Send(new MarkNotificationsReadCommand(CurrentAccountId, id));
        return NoContent();
    }

    /// <summary>Пометить все уведомления прочитанными</summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await Mediator.Send(new MarkNotificationsReadCommand(CurrentAccountId, null));
        return NoContent();
    }
}
