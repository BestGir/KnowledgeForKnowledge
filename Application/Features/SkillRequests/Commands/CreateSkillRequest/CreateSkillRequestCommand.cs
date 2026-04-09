using MediatR;

namespace Application.Features.SkillRequests.Commands.CreateSkillRequest;

public record CreateSkillRequestCommand(Guid AccountID, Guid SkillID, string Title, string? Details) : IRequest<Guid>;
