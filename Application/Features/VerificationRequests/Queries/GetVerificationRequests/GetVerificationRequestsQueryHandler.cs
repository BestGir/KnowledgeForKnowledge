using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.VerificationRequests.Queries.GetVerificationRequests;

public class GetVerificationRequestsQueryHandler
    : IRequestHandler<GetVerificationRequestsQuery, PagedResult<VerificationRequestDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVerificationRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<VerificationRequestDto>> Handle(
        GetVerificationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.VerificationRequests.AsNoTracking();

        if (request.AccountID.HasValue)
            query = query.Where(r => r.AccountID == request.AccountID.Value);

        if (request.Status.HasValue)
            query = query.Where(r => r.Status == request.Status.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new VerificationRequestDto(
                r.RequestID,
                r.AccountID,
                r.RequestType,
                r.Status,
                r.ProofID))
            .ToListAsync(cancellationToken);

        return PagedResult<VerificationRequestDto>.Create(items, total, request.Page, request.PageSize);
    }
}
