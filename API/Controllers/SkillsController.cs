using Application.Features.Skills.Commands.CreateSkill;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class SkillsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateSkill([FromBody] CreateSkillCommand command)
    {
        var skillId = await Mediator.Send(command);
        return Ok(skillId);
    }

    // TODO: Add other endpoints
    // [HttpGet]
    // [HttpGet("{id}")]
    // [HttpPut("{id}")]
    // [HttpDelete("{id}")]
}


