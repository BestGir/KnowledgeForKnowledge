using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.VerificationRequests.Queries.GetVerificationRequests;

/// <summary>
/// Список заявок на верификацию.
/// accountId = null → Admin видит все; иначе — только свои.
/// </summary>
public record GetVerificationRequestsQuery(
    Guid? AccountID,
    VerificationStatus? Status,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<VerificationRequestDto>>;

public record VerificationRequestDto(
    Guid RequestID,
    Guid AccountID,
    VerificationRequestType RequestType,
    VerificationStatus Status,
    Guid? ProofID
);
