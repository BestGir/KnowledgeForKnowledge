namespace Domain.Entities;

/// <summary>
/// Отзыв об участнике сделки. Оставляется после завершения сделки.
/// Один автор — один отзыв на одну сделку.
/// </summary>
public class Review
{
    public Guid ReviewID { get; set; }

    public Guid DealID { get; set; }

    /// <summary>Кто оставил отзыв</summary>
    public Guid AuthorID { get; set; }

    /// <summary>О ком отзыв</summary>
    public Guid TargetID { get; set; }

    /// <summary>Рейтинг 1–5</summary>
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Deal Deal { get; set; } = null!;
    public Account Author { get; set; } = null!;
    public Account Target { get; set; } = null!;
}
