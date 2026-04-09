using Domain.Enums;

namespace Domain.Entities;

public class Notification
{
    public Guid NotificationID { get; set; }
    public Guid AccountID { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    /// <summary>ID связанной сущности (Deal, Application и т.д.)</summary>
    public Guid? RelatedEntityId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Account Account { get; set; } = null!;
}
