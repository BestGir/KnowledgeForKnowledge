using MediatR;

namespace Application.Features.SkillRequests.Commands.DeleteSkillRequest;

public record DeleteSkillRequestCommand(Guid RequestID, Guid AccountID) : IRequest;
