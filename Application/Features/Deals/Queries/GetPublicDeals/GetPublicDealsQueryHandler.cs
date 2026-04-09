using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deals.Queries.GetPublicDeals;

public class GetPublicDealsQueryHandler : IRequestHandler<GetPublicDealsQuery, GetPublicDealsResult>
{
    private readonly IApplicationDbContext _context;

    public GetPublicDealsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<GetPublicDealsResult> Handle(GetPublicDealsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Deals
            .Include(d => d.Initiator).ThenInclude(a => a.UserProfile)
            .Include(d => d.Partner).ThenInclude(a => a.UserProfile)
            .Include(d => d.Application).ThenInclude(a => a.SkillOffer)
            .Include(d => d.Application).ThenInclude(a => a.SkillRequest)
            .Where(d =>
                (d.InitiatorID == request.TargetAccountID || d.PartnerID == request.TargetAccountID)
                && (d.Status == DealStatus.Completed || d.Status == DealStatus.Cancelled))
            .OrderByDescending(d => d.CreatedAt);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new PublicDealDto(
                d.DealID,
                // Показываем имя другого участника относительно TargetAccountID
                d.InitiatorID == request.TargetAccountID
                    ? (d.Partner.UserProfile != null ? d.Partner.UserProfile.FullName : d.Partner.Login)
                    : (d.Initiator.UserProfile != null ? d.Initiator.UserProfile.FullName : d.Initiator.Login),
                d.Status.ToString(),
                d.CreatedAt,
                d.CompletedAt,
                d.Application.SkillOffer != null ? d.Application.SkillOffer.Title : null,
                d.Application.SkillRequest != null ? d.Application.SkillRequest.Title : null))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new GetPublicDealsResult(items, total, request.Page, request.PageSize, totalPages);
    }
}
