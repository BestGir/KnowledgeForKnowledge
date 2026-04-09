using System.Security.Claims;
using Application.Features.Matches.Queries.GetMatches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Умный подбор: находит пользователей, с которыми возможен обмен навыками.
    /// Алгоритм: пересекаем мои навыки с их запросами и наоборот.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMatches()
    {
        var result = await Mediator.Send(new GetMatchesQuery(CurrentAccountId));
        return Ok(result);
    }
}
