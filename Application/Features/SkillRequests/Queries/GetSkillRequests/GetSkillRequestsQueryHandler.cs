using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillRequests.Queries.GetSkillRequests;

public class GetSkillRequestsQueryHandler : IRequestHandler<GetSkillRequestsQuery, PagedResult<SkillRequestDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSkillRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SkillRequestDto>> Handle(GetSkillRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SkillRequests
            .Include(r => r.SkillsCatalog)
            .Include(r => r.Account).ThenInclude(a => a.UserProfile)
            .AsQueryable();

        if (request.AccountID.HasValue)
            query = query.Where(r => r.AccountID == request.AccountID.Value);

        if (request.Status.HasValue)
            query = query.Where(r => r.Status == request.Status.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new SkillRequestDto(
                r.RequestID,
                r.AccountID,
                r.Account.UserProfile != null ? r.Account.UserProfile.FullName : r.Account.Login,
                r.Account.UserProfile != null ? r.Account.UserProfile.PhotoURL : null,
                r.SkillID,
                r.SkillsCatalog.SkillName,
                r.Title,
                r.Details,
                r.Status))
            .ToListAsync(cancellationToken);

        return PagedResult<SkillRequestDto>.Create(items, total, request.Page, request.PageSize);
    }
}
