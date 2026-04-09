using MediatR;

namespace Application.Features.Applications.Commands.CreateApplication;

/// <summary>
/// Отклик на SkillOffer (OfferID заполнен) или на SkillRequest (SkillRequestID заполнен).
/// Ровно одно из двух полей должно быть не null.
/// </summary>
public record CreateApplicationCommand(
    Guid ApplicantID,
    Guid? OfferID,
    Guid? SkillRequestID,
    string? Message) : IRequest<Guid>;
