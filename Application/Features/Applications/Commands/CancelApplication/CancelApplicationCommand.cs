using MediatR;

namespace Application.Features.Applications.Commands.CancelApplication;

/// <summary>Отозвать свой отклик (только пока статус Pending).</summary>
public record CancelApplicationCommand(Guid ApplicationID, Guid ApplicantID) : IRequest;
