using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Skills.Queries.GetSkills;

public class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, PagedResult<SkillDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSkillsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SkillsCatalog.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.SkillName.ToLower().Contains(request.Search.ToLower()));

        if (request.Epithet.HasValue)
            query = query.Where(s => s.Epithet == request.Epithet.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.SkillName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SkillDto(s.SkillID, s.SkillName, s.Epithet))
            .ToListAsync(cancellationToken);

        return PagedResult<SkillDto>.Create(items, total, request.Page, request.PageSize);
    }
}
