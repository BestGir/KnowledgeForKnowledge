using Domain.Enums;
using MediatR;

namespace Application.Features.VerificationRequests.Commands.SubmitVerificationRequest;

/// <summary>Подать заявку на верификацию навыка или аккаунта.</summary>
public record SubmitVerificationRequestCommand(
    Guid AccountID,
    VerificationRequestType RequestType,
    Guid? ProofID
) : IRequest<Guid>;
