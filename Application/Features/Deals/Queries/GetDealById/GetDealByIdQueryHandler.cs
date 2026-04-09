using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deals.Queries.GetDealById;

public class GetDealByIdQueryHandler : IRequestHandler<GetDealByIdQuery, DealDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetDealByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DealDetailDto> Handle(GetDealByIdQuery request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals
            .Include(d => d.Initiator).ThenInclude(a => a.UserProfile)
            .Include(d => d.Partner).ThenInclude(a => a.UserProfile)
            .Include(d => d.Application).ThenInclude(a => a.SkillOffer)
            .Include(d => d.Application).ThenInclude(a => a.SkillRequest)
            .Include(d => d.Reviews).ThenInclude(r => r.Author).ThenInclude(a => a.UserProfile)
            .Include(d => d.Reviews).ThenInclude(r => r.Target).ThenInclude(a => a.UserProfile)
            .FirstOrDefaultAsync(d => d.DealID == request.DealID, cancellationToken);

        if (deal is null)
            throw new NotFoundException(nameof(Domain.Entities.Deal), request.DealID);

        if (deal.InitiatorID != request.RequestingAccountID && deal.PartnerID != request.RequestingAccountID)
            throw new UnauthorizedAccessException("Нет доступа к этой сделке.");

        var reviews = deal.Reviews.Select(r => new ReviewDto(
            r.ReviewID,
            r.AuthorID,
            r.Author.UserProfile?.FullName ?? r.Author.Login,
            r.TargetID,
            r.Target.UserProfile?.FullName ?? r.Target.Login,
            r.Rating,
            r.Comment,
            r.CreatedAt)).ToList();

        return new DealDetailDto(
            deal.DealID,
            deal.ApplicationID,
            deal.InitiatorID,
            deal.Initiator.UserProfile?.FullName ?? deal.Initiator.Login,
            deal.PartnerID,
            deal.Partner.UserProfile?.FullName ?? deal.Partner.Login,
            deal.Status.ToString(),
            deal.CreatedAt,
            deal.CompletedAt,
            deal.Application.SkillOffer?.Title,
            deal.Application.SkillRequest?.Title,
            reviews);
    }
}
