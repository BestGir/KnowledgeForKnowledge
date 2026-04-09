using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.SkillOffers.Queries.GetSkillOffers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillOffers.Queries.GetSkillOfferById;

public class GetSkillOfferByIdQueryHandler : IRequestHandler<GetSkillOfferByIdQuery, SkillOfferDto>
{
    private readonly IApplicationDbContext _context;

    public GetSkillOfferByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillOfferDto> Handle(GetSkillOfferByIdQuery request, CancellationToken cancellationToken)
    {
        var offer = await _context.SkillOffers
            .Include(o => o.SkillsCatalog)
            .Include(o => o.Account).ThenInclude(a => a.UserProfile)
            .Where(o => o.OfferID == request.OfferID)
            .Select(o => new SkillOfferDto(
                o.OfferID,
                o.AccountID,
                o.Account.UserProfile != null ? o.Account.UserProfile.FullName : o.Account.Login,
                o.Account.UserProfile != null ? o.Account.UserProfile.PhotoURL : null,
                o.SkillID,
                o.SkillsCatalog.SkillName,
                o.Title,
                o.Details,
                o.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

        if (offer is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillOffer), request.OfferID);

        return offer;
    }
}
