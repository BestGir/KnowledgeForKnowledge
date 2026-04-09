using Application.Features.SkillRequests.Queries.GetSkillRequests;
using MediatR;

namespace Application.Features.SkillRequests.Queries.GetSkillRequestById;

public record GetSkillRequestByIdQuery(Guid RequestID) : IRequest<SkillRequestDto>;
