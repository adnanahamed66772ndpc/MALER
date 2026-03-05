namespace MailerApp.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? UserOrSystem { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
