using System.Security.Claims;
using Application.Features.Education.Commands.AddEducation;
using Application.Features.Education.Commands.DeleteEducation;
using Application.Features.Education.Queries.GetEducations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/education")]
[Authorize]
public class EducationController : BaseController
{
    /// <summary>Список записей об образовании пользователя</summary>
    [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> GetEducations(Guid accountId)
    {
        var result = await Mediator.Send(new GetEducationsQuery(accountId));
        return Ok(result);
    }

    /// <summary>Добавить запись об образовании</summary>
    [HttpPost]
    public async Task<IActionResult> AddEducation([FromBody] AddEducationRequest request)
    {
        var accountId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await Mediator.Send(new AddEducationCommand(
            accountId, request.InstitutionName, request.DegreeField, request.YearCompleted));
        return Created($"/api/education/{accountId}", new { id });
    }

    /// <summary>Удалить запись об образовании</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEducation(Guid id)
    {
        var accountId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await Mediator.Send(new DeleteEducationCommand(id, accountId));
        return NoContent();
    }
}

public class AddEducationRequest
{
    public string InstitutionName { get; init; } = string.Empty;
    public string? DegreeField { get; init; }
    public int? YearCompleted { get; init; }
}
