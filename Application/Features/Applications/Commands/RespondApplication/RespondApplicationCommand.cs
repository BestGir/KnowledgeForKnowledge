using Domain.Enums;
using MediatR;

namespace Application.Features.Applications.Commands.RespondApplication;

/// <summary>Принять или отклонить заявку (только владелец предложения)</summary>
public record RespondApplicationCommand(Guid ApplicationID, Guid OwnerAccountID, ApplicationStatus NewStatus) : IRequest;
