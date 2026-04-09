using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Skills.Queries.GetSkills;

public record GetSkillsQuery(
    string? Search,
    SkillEpithet? Epithet,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<SkillDto>>;

public record SkillDto(Guid SkillID, string SkillName, SkillEpithet Epithet);
