using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Proofs.Queries.GetProofs;

public class GetProofsQueryHandler : IRequestHandler<GetProofsQuery, List<ProofDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProofsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProofDto>> Handle(GetProofsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Proofs
            .Include(p => p.SkillsCatalog)
            .Where(p => p.AccountID == request.AccountID)
            .Select(p => new ProofDto(
                p.ProofID,
                p.SkillID,
                p.SkillsCatalog != null ? p.SkillsCatalog.SkillName : null,
                p.FileURL,
                p.IsVerified))
            .ToListAsync(cancellationToken);
    }
}
