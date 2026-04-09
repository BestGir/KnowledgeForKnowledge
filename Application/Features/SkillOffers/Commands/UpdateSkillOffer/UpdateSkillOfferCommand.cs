using MediatR;

namespace Application.Features.SkillOffers.Commands.UpdateSkillOffer;

public record UpdateSkillOfferCommand(Guid OfferID, Guid AccountID, string Title, string? Details, bool IsActive) : IRequest;
