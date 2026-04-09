using Application.Features.Skills.Commands.CreateSkill;
using Application.Features.Skills.Commands.DeleteSkill;
using Application.Features.Skills.Queries.GetSkills;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/skills")]
public class SkillsController : BaseController
{
    /// <summary>Получить каталог навыков с поиском и фильтрацией</summary>
    [HttpGet]
    public async Task<IActionResult> GetSkills(
        [FromQuery] string? search,
        [FromQuery] SkillEpithet? epithet,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetSkillsQuery(search, epithet, page, pageSize));
        return Ok(result);
    }

    /// <summary>Добавить новый навык в каталог (только Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillCommand command)
    {
        var skillId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetSkills), new { }, new { id = skillId });
    }

    /// <summary>Удалить навык из каталога (только Admin)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSkill(Guid id)
    {
        await Mediator.Send(new DeleteSkillCommand(id));
        return NoContent();
    }
}
