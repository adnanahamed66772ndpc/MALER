using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class SuppressionEntry
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public SuppressionReason Reason { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}
