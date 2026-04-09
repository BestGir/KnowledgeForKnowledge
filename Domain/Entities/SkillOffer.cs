namespace Domain.Entities;

public class SkillOffer
{
    public Guid OfferID { get; set; }
    public Guid AccountID { get; set; }
    public Guid SkillID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Details { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Account Account { get; set; } = null!;
    public SkillsCatalog SkillsCatalog { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}


