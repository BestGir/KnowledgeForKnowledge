using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillOffers.Commands.DeleteSkillOffer;

public class DeleteSkillOfferCommandHandler : IRequestHandler<DeleteSkillOfferCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSkillOfferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSkillOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.SkillOffers
            .FirstOrDefaultAsync(o => o.OfferID == request.OfferID, cancellationToken);

        if (offer is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillOffer), request.OfferID);

        if (offer.AccountID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к удалению этого предложения.");

        _context.SkillOffers.Remove(offer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
