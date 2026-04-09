using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Matches.Queries.GetMatches;

public class GetMatchesQueryHandler : IRequestHandler<GetMatchesQuery, List<MatchDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMatchesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<MatchDto>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        // Мои навыки и мои запросы — два лёгких запроса
        var mySkillIds = await _context.UserSkills
            .Where(us => us.AccountID == request.AccountID)
            .Select(us => us.SkillID)
            .ToListAsync(cancellationToken);

        var myWantedSkillIds = await _context.SkillRequests
            .Where(sr => sr.AccountID == request.AccountID && sr.Status == RequestStatus.Open)
            .Select(sr => sr.SkillID)
            .ToListAsync(cancellationToken);

        if (mySkillIds.Count == 0 && myWantedSkillIds.Count == 0)
            return [];

        // --- Сторона 1: кандидаты у которых есть запросы на мои навыки ---
        var candidatesWantMySkills = mySkillIds.Count > 0
            ? await _context.SkillRequests
                .Where(sr => sr.Status == RequestStatus.Open
                          && sr.AccountID != request.AccountID
                          && mySkillIds.Contains(sr.SkillID))
                .Select(sr => new { sr.AccountID, sr.SkillID, sr.RequestID, sr.Title,
                                    SkillName = sr.SkillsCatalog.SkillName })
                .ToListAsync(cancellationToken)
            : [];

        // --- Сторона 2: кандидаты у которых есть навыки которые я хочу ---
        var candidatesHaveMyWanted = myWantedSkillIds.Count > 0
            ? await _context.UserSkills
                .Where(us => us.AccountID != request.AccountID
                          && myWantedSkillIds.Contains(us.SkillID))
                .Select(us => new { us.AccountID, us.SkillID,
                                    SkillName = us.SkillsCatalog.SkillName })
                .ToListAsync(cancellationToken)
            : [];

        // Уникальные ID кандидатов
        var candidateIds = candidatesWantMySkills.Select(x => x.AccountID)
            .Union(candidatesHaveMyWanted.Select(x => x.AccountID))
            .ToHashSet();

        if (candidateIds.Count == 0) return [];

        // Одним запросом грузим имена кандидатов
        var profiles = await _context.Accounts
            .Where(a => candidateIds.Contains(a.AccountID))
            .Select(a => new { a.AccountID, a.Login,
                               FullName = a.UserProfile != null ? a.UserProfile.FullName : a.Login })
            .ToListAsync(cancellationToken);

        // Группируем данные по кандидату
        var wantMySkillsByAccount = candidatesWantMySkills
            .GroupBy(x => x.AccountID)
            .ToDictionary(g => g.Key, g => g.ToList());

        var haveMyWantedByAccount = candidatesHaveMyWanted
            .GroupBy(x => x.AccountID)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = profiles.Select(p =>
        {
            var theirRequests = wantMySkillsByAccount.TryGetValue(p.AccountID, out var reqs)
                ? reqs.Select(r => new MatchRequestDto(r.RequestID, r.Title, r.SkillName)).ToList()
                : [];

            var theyHaveIWant = haveMyWantedByAccount.TryGetValue(p.AccountID, out var skills)
                ? skills.Select(s => s.SkillName).Distinct().ToList()
                : [];

            var iHaveTheyWant = wantMySkillsByAccount.TryGetValue(p.AccountID, out var want)
                ? want.Select(r => r.SkillName).Distinct().ToList()
                : [];

            return new MatchDto(p.AccountID, p.FullName, theyHaveIWant, iHaveTheyWant, theirRequests);
        }).ToList();

        return result;
    }
}
