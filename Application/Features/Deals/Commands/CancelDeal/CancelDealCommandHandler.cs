using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deals.Commands.CancelDeal;

public class CancelDealCommandHandler : IRequestHandler<CancelDealCommand>
{
    private readonly IApplicationDbContext _context;

    public CancelDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelDealCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals
            .FirstOrDefaultAsync(d => d.DealID == request.DealID, cancellationToken);

        if (deal is null)
            throw new NotFoundException(nameof(Domain.Entities.Deal), request.DealID);

        if (deal.InitiatorID != request.AccountID && deal.PartnerID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к этой сделке.");

        if (deal.Status == DealStatus.Completed)
            throw new InvalidOperationException("Нельзя отменить завершённую сделку.");

        if (deal.Status == DealStatus.Cancelled)
            throw new InvalidOperationException("Сделка уже отменена.");

        deal.Status = DealStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
