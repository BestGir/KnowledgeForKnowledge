using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.Queries.GetReviews;

public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, GetReviewsResult>
{
    private readonly IApplicationDbContext _context;

    public GetReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetReviewsResult> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .Include(r => r.Author).ThenInclude(a => a.UserProfile)
            .Where(r => r.TargetID == request.TargetAccountID)
            .OrderByDescending(r => r.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var avgRating = total > 0 ? await query.AverageAsync(r => (double)r.Rating, cancellationToken) : 0;

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewSummaryDto(
                r.ReviewID,
                r.DealID,
                r.AuthorID,
                r.Author.UserProfile != null ? r.Author.UserProfile.FullName : r.Author.Login,
                r.Rating,
                r.Comment,
                r.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new GetReviewsResult(items, total, Math.Round(avgRating, 2), request.Page, request.PageSize, totalPages);
    }
}
