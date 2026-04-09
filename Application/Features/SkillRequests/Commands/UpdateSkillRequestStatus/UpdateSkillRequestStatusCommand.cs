using Domain.Enums;
using MediatR;

namespace Application.Features.SkillRequests.Commands.UpdateSkillRequestStatus;

public record UpdateSkillRequestStatusCommand(Guid RequestID, Guid AccountID, RequestStatus Status) : IRequest;
