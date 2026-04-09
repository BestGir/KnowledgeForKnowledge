using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateReviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new InvalidOperationException("Рейтинг должен быть от 1 до 5.");

        var deal = await _context.Deals
            .FirstOrDefaultAsync(d => d.DealID == request.DealID, cancellationToken);

        if (deal is null)
            throw new NotFoundException(nameof(Domain.Entities.Deal), request.DealID);

        if (deal.Status != DealStatus.Completed)
            throw new InvalidOperationException("Отзыв можно оставить только после завершения сделки.");

        if (deal.InitiatorID != request.AuthorID && deal.PartnerID != request.AuthorID)
            throw new UnauthorizedAccessException("Вы не участник этой сделки.");

        // Определяем, о ком оставляем отзыв
        var targetId = deal.InitiatorID == request.AuthorID ? deal.PartnerID : deal.InitiatorID;

        // Проверяем, не оставлял ли уже отзыв
        var existing = await _context.Reviews
            .AnyAsync(r => r.DealID == request.DealID && r.AuthorID == request.AuthorID, cancellationToken);
        if (existing)
            throw new InvalidOperationException("Вы уже оставили отзыв по этой сделке.");

        var review = new Domain.Entities.Review
        {
            ReviewID = Guid.NewGuid(),
            DealID = request.DealID,
            AuthorID = request.AuthorID,
            TargetID = targetId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return review.ReviewID;
    }
}
