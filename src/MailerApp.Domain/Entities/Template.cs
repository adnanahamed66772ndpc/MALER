namespace MailerApp.Domain.Entities;

public class Template
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
