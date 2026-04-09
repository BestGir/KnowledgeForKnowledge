using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Отклик пользователя на предложение (SkillOffer) или запрос (SkillRequest).
/// Ровно одно из полей OfferID / SkillRequestID должно быть заполнено.
/// </summary>
public class Application
{
    public Guid ApplicationID { get; set; }

    /// <summary>Кто откликнулся</summary>
    public Guid ApplicantID { get; set; }

    /// <summary>На чьё предложение (если отклик на SkillOffer)</summary>
    public Guid? OfferID { get; set; }

    /// <summary>На чей запрос (если отклик на SkillRequest)</summary>
    public Guid? SkillRequestID { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Account Applicant { get; set; } = null!;
    public SkillOffer? SkillOffer { get; set; }
    public SkillRequest? SkillRequest { get; set; }
    public Deal? Deal { get; set; }
}
