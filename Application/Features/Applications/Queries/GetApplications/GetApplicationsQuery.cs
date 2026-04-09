using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Applications.Queries.GetApplications;

public record GetApplicationsQuery(
    Guid CurrentAccountID,
    ApplicationQueryType QueryType,
    ApplicationStatus? Status,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<ApplicationDto>>;

/// <summary>
/// Incoming — «заявки для ответа» (на мои предложения)
/// Outgoing — мои отклики
/// Processed — обработанные (Accepted/Rejected)
/// </summary>
public enum ApplicationQueryType { Incoming, Outgoing, Processed }

public record ApplicationDto(
    Guid ApplicationID,
    Guid ApplicantID,
    string ApplicantName,
    string? ApplicantTelegramID,
    Guid? OfferID,
    string? OfferTitle,
    Guid? SkillRequestID,
    string? RequestTitle,
    string? SkillName,
    ApplicationStatus Status,
    string? Message,
    DateTime CreatedAt
);
