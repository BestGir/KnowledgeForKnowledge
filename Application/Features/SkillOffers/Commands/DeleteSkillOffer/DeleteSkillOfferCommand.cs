using MediatR;

namespace Application.Features.SkillOffers.Commands.DeleteSkillOffer;

public record DeleteSkillOfferCommand(Guid OfferID, Guid AccountID) : IRequest;
