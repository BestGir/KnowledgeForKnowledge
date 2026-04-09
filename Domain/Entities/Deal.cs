using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Сделка — создаётся автоматически при принятии отклика.
/// Представляет договорённость об обмене навыками между двумя пользователями.
/// </summary>
public class Deal
{
    public Guid DealID { get; set; }

    /// <summary>Заявка, из которой возникла сделка</summary>
    public Guid ApplicationID { get; set; }

    /// <summary>Кто подал отклик (инициатор)</summary>
    public Guid InitiatorID { get; set; }

    /// <summary>Владелец предложения/запроса (партнёр)</summary>
    public Guid PartnerID { get; set; }

    public DealStatus Status { get; set; } = DealStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public Application Application { get; set; } = null!;
    public Account Initiator { get; set; } = null!;
    public Account Partner { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
