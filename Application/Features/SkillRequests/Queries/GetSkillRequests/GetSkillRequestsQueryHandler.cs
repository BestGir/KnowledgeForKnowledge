using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillRequests.Queries.GetSkillRequests;

public class GetSkillRequestsQueryHandler : IRequestHandler<GetSkillRequestsQuery, PagedResult<SkillRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private static readonly IReadOnlyDictionary<SkillEpithet, string[]> EpithetSearchTerms =
        new Dictionary<SkillEpithet, string[]>
        {
            [SkillEpithet.IT] = ["it", "ит", "айти", "программирование", "разработка"],
            [SkillEpithet.Design] = ["design", "дизайн"],
            [SkillEpithet.Cooking] = ["cooking", "кулинария", "готовка"],
            [SkillEpithet.Language] = ["language", "язык", "языки"],
            [SkillEpithet.Music] = ["music", "музыка"],
            [SkillEpithet.Sports] = ["sports", "спорт"],
            [SkillEpithet.Business] = ["business", "бизнес"],
            [SkillEpithet.Education] = ["education", "образование", "обучение"],
            [SkillEpithet.Healthcare] = ["healthcare", "здоровье", "медицина"],
            [SkillEpithet.Other] = ["other", "другое"],
        };

    public GetSkillRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SkillRequestDto>> Handle(GetSkillRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SkillRequests
            .Include(skillRequest => skillRequest.SkillsCatalog)
            .Include(skillRequest => skillRequest.Account)
                .ThenInclude(account => account.UserProfile)
            .AsQueryable();

        if (request.SkillID.HasValue)
            query = query.Where(skillRequest => skillRequest.SkillID == request.SkillID.Value);

        if (request.AccountID.HasValue)
            query = query.Where(skillRequest => skillRequest.AccountID == request.AccountID.Value);

        if (request.ExcludeAccountID.HasValue)
            query = query.Where(skillRequest => skillRequest.AccountID != request.ExcludeAccountID.Value);

        if (request.Status.HasValue)
            query = query.Where(skillRequest => skillRequest.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            var epithetMatches = ResolveEpithetMatches(search);
            var searchTextFields = search.Length >= 3;
            query = query.Where(skillRequest =>
                skillRequest.SkillsCatalog.SkillName.ToLower().Contains(search) ||
                epithetMatches.Contains(skillRequest.SkillsCatalog.Epithet) ||
                (searchTextFields &&
                    (skillRequest.Title.ToLower().Contains(search) ||
                     (skillRequest.Details != null && skillRequest.Details.ToLower().Contains(search)) ||
                     (skillRequest.Account.UserProfile != null && skillRequest.Account.UserProfile.FullName.ToLower().Contains(search)) ||
                     skillRequest.Account.Login.ToLower().Contains(search))));
        }

        if (!string.IsNullOrWhiteSpace(request.AuthorSearch))
        {
            var authorSearch = request.AuthorSearch.Trim().ToLower();
            query = query.Where(skillRequest =>
                (skillRequest.Account.UserProfile != null && skillRequest.Account.UserProfile.FullName.ToLower().Contains(authorSearch)) ||
                skillRequest.Account.Login.ToLower().Contains(authorSearch));
        }

        if (!string.IsNullOrWhiteSpace(request.TitleSearch))
        {
            var titleSearch = request.TitleSearch.Trim().ToLower();
            query = query.Where(skillRequest => skillRequest.Title.ToLower().Contains(titleSearch));
        }

        if (!string.IsNullOrWhiteSpace(request.EpithetSearch))
        {
            var epithetMatches = ResolveEpithetMatches(request.EpithetSearch.Trim().ToLower());
            query = query.Where(skillRequest => epithetMatches.Contains(skillRequest.SkillsCatalog.Epithet));
        }

        if (request.HelperAccountID.HasValue && request.CanHelp.HasValue)
        {
            if (request.CanHelp.Value)
            {
                query = query.Where(skillRequest =>
                    _context.UserSkills.Any(
                        userSkill =>
                            userSkill.AccountID == request.HelperAccountID.Value &&
                            userSkill.SkillID == skillRequest.SkillID));
            }
            else
            {
                query = query.Where(skillRequest =>
                    !_context.UserSkills.Any(
                        userSkill =>
                            userSkill.AccountID == request.HelperAccountID.Value &&
                            userSkill.SkillID == skillRequest.SkillID));
            }
        }

        if (request.HelperAccountID.HasValue && request.RequireBarter == true)
        {
            query = query.Where(skillRequest =>
                _context.UserSkills.Any(authorSkill =>
                    authorSkill.AccountID == skillRequest.AccountID &&
                    !_context.UserSkills.Any(viewerSkill =>
                        viewerSkill.AccountID == request.HelperAccountID.Value &&
                        viewerSkill.SkillID == authorSkill.SkillID) &&
                    _context.SkillRequests.Any(viewerRequest =>
                        viewerRequest.AccountID == request.HelperAccountID.Value &&
                        viewerRequest.Status == RequestStatus.Open &&
                        viewerRequest.SkillID == authorSkill.SkillID)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(skillRequest => skillRequest.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(skillRequest => new SkillRequestDto(
                skillRequest.RequestID,
                skillRequest.AccountID,
                skillRequest.Account.UserProfile != null ? skillRequest.Account.UserProfile.FullName : skillRequest.Account.Login,
                skillRequest.Account.UserProfile != null ? skillRequest.Account.UserProfile.PhotoURL : null,
                skillRequest.SkillID,
                skillRequest.SkillsCatalog.SkillName,
                skillRequest.SkillsCatalog.Epithet,
                skillRequest.Title,
                skillRequest.Details,
                skillRequest.Status))
            .ToListAsync(cancellationToken);

        return PagedResult<SkillRequestDto>.Create(items, total, request.Page, request.PageSize);
    }

    private static SkillEpithet[] ResolveEpithetMatches(string search)
    {
        return EpithetSearchTerms
            .Where(pair => pair.Value.Any(term => term.Contains(search) || search.Contains(term)))
            .Select(pair => pair.Key)
            .ToArray();
    }
}
