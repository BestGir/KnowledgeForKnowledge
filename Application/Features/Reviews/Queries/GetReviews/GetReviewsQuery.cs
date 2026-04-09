using MediatR;

namespace Application.Features.Reviews.Queries.GetReviews;

public record GetReviewsQuery(Guid TargetAccountID, int Page = 1, int PageSize = 20) : IRequest<GetReviewsResult>;

public record ReviewSummaryDto(
    Guid ReviewID,
    Guid DealID,
    Guid AuthorID,
    string AuthorName,
    int Rating,
    string? Comment,
    DateTime CreatedAt);

public record GetReviewsResult(List<ReviewSummaryDto> Items, int TotalCount, double AverageRating, int Page, int PageSize, int TotalPages);
