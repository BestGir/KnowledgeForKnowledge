using Application.Features.SkillOffers.Queries.GetSkillOffers;
using MediatR;

namespace Application.Features.SkillOffers.Queries.GetSkillOfferById;

public record GetSkillOfferByIdQuery(Guid OfferID) : IRequest<SkillOfferDto>;
