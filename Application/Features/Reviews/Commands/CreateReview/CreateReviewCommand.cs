using MediatR;

namespace Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(Guid DealID, Guid AuthorID, int Rating, string? Comment) : IRequest<Guid>;
