using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deals.Commands.CompleteDeal;

public class CompleteDealCommandHandler : IRequestHandler<CompleteDealCommand>
{
    private readonly IApplicationDbContext _context;

    public CompleteDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CompleteDealCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals
            .FirstOrDefaultAsync(d => d.DealID == request.DealID, cancellationToken);

        if (deal is null)
            throw new NotFoundException(nameof(Domain.Entities.Deal), request.DealID);

        if (deal.InitiatorID != request.AccountID && deal.PartnerID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к этой сделке.");

        if (deal.Status == DealStatus.Completed)
            throw new InvalidOperationException("Сделка уже завершена.");

        if (deal.Status == DealStatus.Cancelled)
            throw new InvalidOperationException("Нельзя завершить отменённую сделку.");

        var isInitiator = deal.InitiatorID == request.AccountID;

        deal.Status = deal.Status switch
        {
            DealStatus.Active when isInitiator => DealStatus.CompletedByInitiator,
            DealStatus.Active => DealStatus.CompletedByPartner,
            DealStatus.CompletedByInitiator when !isInitiator => DealStatus.Completed,
            DealStatus.CompletedByPartner when isInitiator => DealStatus.Completed,
            _ => throw new InvalidOperationException("Вы уже отметили сделку завершённой.")
        };

        if (deal.Status == DealStatus.Completed)
            deal.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
