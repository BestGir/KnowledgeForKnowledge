using MediatR;

namespace Application.Features.UserSkills.Commands.RemoveUserSkill;

public record RemoveUserSkillCommand(Guid AccountID, Guid SkillID) : IRequest;
