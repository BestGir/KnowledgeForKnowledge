using MediatR;

namespace Application.Features.Matches.Queries.GetMatches;

public record GetMatchesQuery(Guid AccountID) : IRequest<List<MatchDto>>;

/// <summary>
/// Потенциальный матч: другой пользователь, у которого есть совпадение навыков.
/// </summary>
public record MatchDto(
    Guid AccountID,
    string FullName,
    /// <summary>Навыки которые я хочу выучить и он умеет (из его UserSkills)</summary>
    List<string> SkillsTheyHaveThatIWant,
    /// <summary>Навыки которые он хочет выучить и я умею (из моих UserSkills)</summary>
    List<string> SkillsIHaveThatTheyWant,
    /// <summary>Запросы другого пользователя, на которые я могу откликнуться</summary>
    List<MatchRequestDto> TheirRequests);

public record MatchRequestDto(Guid RequestID, string Title, string SkillName);
