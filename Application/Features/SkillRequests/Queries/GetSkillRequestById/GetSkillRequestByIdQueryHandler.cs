using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.SkillRequests.Queries.GetSkillRequests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillRequests.Queries.GetSkillRequestById;

public class GetSkillRequestByIdQueryHandler : IRequestHandler<GetSkillRequestByIdQuery, SkillRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetSkillRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillRequestDto> Handle(GetSkillRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.SkillRequests
            .Include(r => r.SkillsCatalog)
            .Include(r => r.Account).ThenInclude(a => a.UserProfile)
            .Where(r => r.RequestID == request.RequestID)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillRequest), request.RequestID);

        return dto;
    }
}
