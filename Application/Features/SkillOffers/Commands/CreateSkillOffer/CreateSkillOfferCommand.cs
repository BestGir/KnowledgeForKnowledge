using MediatR;

namespace Application.Features.SkillOffers.Commands.CreateSkillOffer;

public record CreateSkillOfferCommand(Guid AccountID, Guid SkillID, string Title, string? Details) : IRequest<Guid>;
