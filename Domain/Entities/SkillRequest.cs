using Domain.Enums;

namespace Domain.Entities;

public class SkillRequest
{
    public Guid RequestID { get; set; }
    public Guid AccountID { get; set; }
    public Guid SkillID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Details { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Account Account { get; set; } = null!;
    public SkillsCatalog SkillsCatalog { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
