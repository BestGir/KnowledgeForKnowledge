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
            .Select(r => new
            {
                r.ReviewID,
                r.DealID,
                r.AuthorID,
                AuthorName = r.Author.UserProfile != null ? r.Author.UserProfile.FullName : r.Author.Login,
                SkillInfo = _context.Applications
                    .Where(application =>
                        application.ApplicationID ==
                        _context.Deals
                            .Where(deal => deal.DealID == r.DealID)
                            .Select(deal => deal.ApplicationID)
                            .FirstOrDefault())
                    .Select(application => new
                    {
                        SkillID = application.SkillOffer != null
                            ? (Guid?)application.SkillOffer.SkillID
                            : application.SkillRequest != null
                                ? (Guid?)application.SkillRequest.SkillID
                                : null,
                        SkillName = application.SkillOffer != null
                            ? application.SkillOffer.SkillsCatalog.SkillName
                            : application.SkillRequest != null
                                ? application.SkillRequest.SkillsCatalog.SkillName
                                : null,
                    })
                    .FirstOrDefault(),
                r.Rating,
                r.Comment,
                r.CreatedAt,
            })
            .Select(review => new ReviewSummaryDto(
                review.ReviewID,
                review.DealID,
                review.AuthorID,
                review.AuthorName,
                review.SkillInfo != null ? review.SkillInfo.SkillID : null,
                review.SkillInfo != null ? review.SkillInfo.SkillName : null,
                review.Rating,
                review.Comment,
                review.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new GetReviewsResult(items, total, Math.Round(avgRating, 2), request.Page, request.PageSize, totalPages);
    }
}
