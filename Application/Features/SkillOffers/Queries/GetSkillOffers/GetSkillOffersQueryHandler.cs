using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillOffers.Queries.GetSkillOffers;

public class GetSkillOffersQueryHandler : IRequestHandler<GetSkillOffersQuery, PagedResult<SkillOfferDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSkillOffersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SkillOfferDto>> Handle(GetSkillOffersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SkillOffers
            .Include(o => o.SkillsCatalog)
            .Include(o => o.Account)
                .ThenInclude(a => a.UserProfile)
            .AsQueryable();

        if (request.SkillID.HasValue)
            query = query.Where(o => o.SkillID == request.SkillID.Value);

        if (request.AccountID.HasValue)
            query = query.Where(o => o.AccountID == request.AccountID.Value);

        if (request.IsActive.HasValue)
            query = query.Where(o => o.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(o =>
                o.Title.ToLower().Contains(s) ||
                (o.Details != null && o.Details.ToLower().Contains(s)) ||
                o.SkillsCatalog.SkillName.ToLower().Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return PagedResult<SkillOfferDto>.Create(items, total, request.Page, request.PageSize);
    }
}
