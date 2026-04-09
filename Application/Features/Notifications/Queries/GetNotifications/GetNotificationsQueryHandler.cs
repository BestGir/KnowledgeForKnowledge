using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, GetNotificationsResult>
{
    private readonly IApplicationDbContext _context;

    public GetNotificationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<GetNotificationsResult> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications
            .Where(n => n.AccountID == request.AccountID);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        var total = await query.CountAsync(cancellationToken);
        var unread = await _context.Notifications
            .CountAsync(n => n.AccountID == request.AccountID && !n.IsRead, cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(
                n.NotificationID,
                n.Type.ToString(),
                n.Message,
                n.IsRead,
                n.RelatedEntityId,
                n.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new GetNotificationsResult(items, total, unread, request.Page, request.PageSize, totalPages);
    }
}
