using MediatR;

namespace Application.Features.Skills.Commands.DeleteSkill;

public record DeleteSkillCommand(Guid SkillID) : IRequest;
