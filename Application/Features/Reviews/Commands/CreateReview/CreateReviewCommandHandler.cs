using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITelegramService _telegram;

    public CreateReviewCommandHandler(IApplicationDbContext context, ITelegramService telegram)
    {
        _context = context;
        _telegram = telegram;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new InvalidOperationException("Рейтинг должен быть от 1 до 5.");

        var deal = await _context.Deals
            .Include(d => d.Initiator)
                .ThenInclude(a => a.UserProfile)
            .Include(d => d.Partner)
            .FirstOrDefaultAsync(d => d.DealID == request.DealID, cancellationToken);

        if (deal is null)
            throw new NotFoundException(nameof(Domain.Entities.Deal), request.DealID);

        if (deal.Status == DealStatus.Cancelled)
            throw new InvalidOperationException("Нельзя оставить отзыв по отменённой сделке.");

        if (deal.InitiatorID != request.AuthorID && deal.PartnerID != request.AuthorID)
            throw new UnauthorizedAccessException("Вы не участник этой сделки.");

        var targetId = deal.InitiatorID == request.AuthorID ? deal.PartnerID : deal.InitiatorID;

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

        var author = deal.InitiatorID == request.AuthorID ? deal.Initiator : deal.Partner;
        var target = deal.InitiatorID == request.AuthorID ? deal.Partner : deal.Initiator;
        var authorName = author.UserProfile?.FullName ?? author.Login;
        var reviewSummary = string.IsNullOrWhiteSpace(request.Comment)
            ? $"{authorName} оставил(а) отзыв: {request.Rating}/5."
            : $"{authorName} оставил(а) отзыв: {request.Rating}/5. Комментарий: {request.Comment.Trim()}";

        _context.Reviews.Add(review);
        _context.Notifications.Add(new Notification
        {
            NotificationID = Guid.NewGuid(),
            AccountID = target.AccountID,
            Type = NotificationType.NewReview,
            Message = reviewSummary,
            RelatedEntityId = review.ReviewID,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(target.TelegramID) && target.NotificationsEnabled)
        {
            var telegramText = string.IsNullOrWhiteSpace(request.Comment)
                ? $"Новый отзыв от {authorName}: {request.Rating}/5."
                : $"Новый отзыв от {authorName}: {request.Rating}/5.\nКомментарий: {request.Comment.Trim()}";

            await _telegram.SendMessageAsync(target.TelegramID, telegramText, cancellationToken);
        }

        return review.ReviewID;
    }
}
