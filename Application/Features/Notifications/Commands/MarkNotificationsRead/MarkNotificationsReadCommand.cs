using MediatR;

namespace Application.Features.Notifications.Commands.MarkNotificationsRead;

/// <summary>
/// NotificationID = null → пометить все непрочитанные.
/// NotificationID задан → пометить одно.
/// </summary>
public record MarkNotificationsReadCommand(Guid AccountID, Guid? NotificationID) : IRequest;
