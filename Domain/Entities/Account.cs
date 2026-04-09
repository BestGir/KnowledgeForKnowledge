namespace Domain.Entities;

public class Account
{
    public Guid AccountID { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? TelegramID { get; set; }

    /// <summary>Временный токен для привязки Telegram через бота</summary>
    public string? TelegramLinkToken { get; set; }

    public bool IsAdmin { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>Счётчик неверных попыток входа (сбрасывается при успехе)</summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>Блокировка до этого времени (null = не заблокирован)</summary>
    public DateTime? LockoutUntil { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public UserProfile? UserProfile { get; set; }
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<Education> Educations { get; set; } = new List<Education>();
    public ICollection<Proof> Proofs { get; set; } = new List<Proof>();
    public ICollection<SkillOffer> SkillOffers { get; set; } = new List<SkillOffer>();
    public ICollection<SkillRequest> SkillRequests { get; set; } = new List<SkillRequest>();
    public ICollection<VerificationRequest> VerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Deal> InitiatedDeals { get; set; } = new List<Deal>();
    public ICollection<Deal> PartnerDeals { get; set; } = new List<Deal>();
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
}
