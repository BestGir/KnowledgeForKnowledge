using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillOffers.Commands.UpdateSkillOffer;

public class UpdateSkillOfferCommandHandler : IRequestHandler<UpdateSkillOfferCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSkillOfferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSkillOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.SkillOffers
            .FirstOrDefaultAsync(o => o.OfferID == request.OfferID, cancellationToken);

        if (offer is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillOffer), request.OfferID);

        if (offer.AccountID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к редактированию этого предложения.");

        offer.Title = request.Title;
        offer.Details = request.Details;
        offer.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
