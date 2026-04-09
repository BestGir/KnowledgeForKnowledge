using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Education.Queries.GetEducations;

public class GetEducationsQueryHandler : IRequestHandler<GetEducationsQuery, List<EducationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEducationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationDto>> Handle(GetEducationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Educations
            .Where(e => e.AccountID == request.AccountID)
            .Select(e => new EducationDto(e.EducationID, e.InstitutionName, e.DegreeField, e.YearCompleted))
            .ToListAsync(cancellationToken);
    }
}
