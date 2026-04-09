using MediatR;

namespace Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(Guid AccountID, bool UnreadOnly = false, int Page = 1, int PageSize = 30)
    : IRequest<GetNotificationsResult>;

public record NotificationDto(
    Guid NotificationID,
    string Type,
    string Message,
    bool IsRead,
    Guid? RelatedEntityId,
    DateTime CreatedAt);

public record GetNotificationsResult(
    List<NotificationDto> Items,
    int TotalCount,
    int UnreadCount,
    int Page,
    int PageSize,
    int TotalPages);
